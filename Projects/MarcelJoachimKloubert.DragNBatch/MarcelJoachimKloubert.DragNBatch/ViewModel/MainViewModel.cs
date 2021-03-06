﻿// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. http://blog.marcel-kloubert.de

using MarcelJoachimKloubert.CLRToolbox.ComponentModel;
using MarcelJoachimKloubert.CLRToolbox.Composition;
using MarcelJoachimKloubert.CLRToolbox.ServiceLocation.Impl;
using MarcelJoachimKloubert.CLRToolbox.Windows.Collections.ObjectModel;
using MarcelJoachimKloubert.DragNBatch.PlugIns;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace MarcelJoachimKloubert.DragNBatch.ViewModel
{
    /// <summary>
    /// The main view model.
    /// </summary>
    public sealed class MainViewModel : NotificationObjectBase
    {
        #region Properties (8)

        /// <summary>
        /// Gets if the application can currently handling files and directories or not.
        /// </summary>
        [ReceiveNotificationFrom("SelectedPlugIn")]
        [ReceiveNotificationFrom("Task")]
        public bool CanHandleFiles
        {
            get
            {
                return this.SelectedPlugIn != null &&
                       this.IsRunning == false;
            }
        }

        public int CurrentStepProgress
        {
            get { return this.Get<int>(); }

            set { this.Set(value); }
        }

        public string DropText
        {
            get { return this.Get<string>(); }

            set { this.Set(value); }
        }

        /// <summary>
        /// Gets if a batch process is currently running or not.
        /// </summary>
        [ReceiveNotificationFrom("Task")]
        public bool IsRunning
        {
            get { return this.Task != null; }
        }

        public int OverallProgress
        {
            get { return this.Get<int>(); }

            set { this.Set(value); }
        }

        /// <summary>
        /// Gets the list of loaded plugins.
        /// </summary>
        public DispatcherObservableCollection<IPlugIn> PlugIns
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the selected plug in.
        /// </summary>
        public IPlugIn SelectedPlugIn
        {
            get { return this.Get<IPlugIn>(); }

            set
            {
                if (this.Set(value))
                {
                    CultureInfo culture = CultureInfo.CurrentUICulture;
                    string newText = null;

                    var curPlugIn = this.Get<IPlugIn>();
                    if (curPlugIn == null)
                    {
                        switch (culture.ThreeLetterISOLanguageName)
                        {
                            case "deu":
                                newText = "Wähle bitte ein PlugIn aus...";
                                break;

                            default:
                                newText = "Please select a PlugIn...";
                                break;
                        }
                    }
                    else
                    {
                        newText = curPlugIn.GetDropText(culture);
                    }

                    this.DropText = newText;
                }
            }
        }

        /// <summary>
        /// Gets the current running task.
        /// </summary>
        public Task Task
        {
            get { return this.Get<Task>(); }

            private set { this.Set(value); }
        }

        #endregion Properties

        #region Methods (4)

        // Public Methods (2) 

        /// <summary>
        /// Reloads the list of plugins.
        /// </summary>
        public void ReloadPlugIns()
        {
            try
            {
                var loadedPlugIns = new List<IPlugIn>();

                var plugInDir = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "PlugIns"));
                if (plugInDir.Exists)
                {
                    foreach (var file in plugInDir.GetFiles("*.dll"))
                    {
                        try
                        {
                            var asmBlob = File.ReadAllBytes(file.FullName);
                            var asm = Assembly.Load(asmBlob);

                            var catalog = new AssemblyCatalog(asm);

                            var ctx = new PlugInContext();
                            ctx.Assembly = asm;
                            ctx.AssemblyFile = file.FullName;

                            var container = new CompositionContainer(catalog,
                                                                     isThreadSafe: true);
                            container.ComposeExportedValue<global::MarcelJoachimKloubert.DragNBatch.PlugIns.IPlugInContext>(ctx);

                            var instances = new MultiInstanceComposer<IPlugIn>(container);
                            instances.RefeshIfNeeded();

                            // service locator
                            {
                                var mefLocator = new ExportProviderServiceLocator(container);

                                var sl = new DelegateServiceLocator(mefLocator);

                                ctx.ServiceLocator = sl;
                            }

                            var initializedPlugIns = new List<IPlugIn>();
                            foreach (var i in instances.Instances)
                            {
                                try
                                {
                                    i.Initialize(ctx);

                                    initializedPlugIns.Add(i);
                                }
                                catch (Exception ex)
                                {
                                    this.OnError(ex);
                                }
                            }

                            ctx.PlugIns = initializedPlugIns.ToArray();
                            loadedPlugIns.AddRange(initializedPlugIns);
                        }
                        catch (Exception ex)
                        {
                            this.OnError(ex);
                        }
                    }
                }

                this.PlugIns.Clear();
                this.PlugIns.AddRange(loadedPlugIns);
            }
            catch (Exception ex)
            {
                this.OnError(ex);
            }
        }

        /// <summary>
        /// Starts handling files.
        /// </summary>
        /// <param name="plugIn">The plugin that should handle files.</param>
        /// <param name="context">The underlying context.</param>
        public bool HandleFiles(IPlugIn plugIn, IHandleFilesContext context)
        {
            bool result = false;
            Exception occuredException = null;

            lock (this._SYNC)
            {
                if (this.IsRunning == false)
                {
                    try
                    {
                        var newTask = CreateHandleFilesTask(this, plugIn, context);
                        this.Task = newTask;

                        newTask.Start();
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        this.Task = null;
                        occuredException = ex;
                    }
                }
            }

            if (occuredException != null)
            {
                this.OnError(occuredException);
            }

            return result;
        }

        // Protected Methods (1) 

        /// <inheriteddoc />
        protected override void OnConstructor()
        {
            this.PlugIns = DispatcherObservableCollection.Create<IPlugIn>();

            this.SelectedPlugIn = null;
        }

        // Private Methods (1) 
        private static Task CreateHandleFilesTask(MainViewModel viewModel, IPlugIn plugIn, IHandleFilesContext ctx)
        {
            return new Task((state) =>
                {
                    MainViewModel vm = (MainViewModel)state;

                    try
                    {
                        plugIn.HandleFiles(ctx);
                    }
                    catch (Exception ex)
                    {
                        vm.OnError(ex);
                    }
                    finally
                    {
                        vm.Task = null;
                    }
                }, state: viewModel
                 , creationOptions: TaskCreationOptions.LongRunning);
        }

        #endregion Methods
    }
}