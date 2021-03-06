﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. http://blog.marcel-kloubert.de

using MarcelJoachimKloubert.CLRToolbox.Data;
using MarcelJoachimKloubert.CLRToolbox.Helpers;
using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Execution.Workflows
{
    #region CLASS: SimpleWorkflowExecutionContext

    /// <summary>
    /// Simple implementation of <see cref="IWorkflowExecutionContext" /> interface.
    /// </summary>
    public class SimpleWorkflowExecutionContext : IWorkflowExecutionContext
    {
        #region Fields (12)

        private bool _cancel;
        private bool _continueOnError;
        private IReadOnlyList<object> _executionArguments;
        private IDictionary<string, object> _globalVars;
        private bool _hasBeenCanceled;
        private long _index;
        private WorkflowActionNoState _next;
        private IDictionary<string, object> _nextVars;
        private IReadOnlyDictionary<string, object> _previousVars;
        private object _result;
        private object _syncRoot;
        private bool _throwErrors;
        private IWorkflow _workflow;
        private IDictionary<string, object> _workflowVars;

        #endregion CLASS: SimpleWorkflowExecutionContext

        #region Properties (16)

        /// <inheriteddoc />
        public bool Cancel
        {
            get { return this._cancel; }

            set { this._cancel = value; }
        }

        /// <inheriteddoc />
        public bool ContinueOnError
        {
            get { return this._continueOnError; }

            set { this._continueOnError = value; }
        }

        /// <inheriteddoc />
        public IReadOnlyList<object> ExecutionArguments
        {
            get { return this._executionArguments; }

            set { this._executionArguments = value; }
        }

        /// <inheriteddoc />
        public IDictionary<string, object> ExecutionVars
        {
            get { return this._globalVars; }

            set { this._globalVars = value; }
        }

        /// <inheriteddoc />
        public bool HasBeenCanceled
        {
            get { return this._hasBeenCanceled; }

            set { this._hasBeenCanceled = value; }
        }

        /// <inheriteddoc />
        public long Index
        {
            get { return this._index; }

            set { this._index = value; }
        }

        /// <inheriteddoc />
        public bool IsFirst
        {
            get { return this.Index == 0; }
        }

        /// <inheriteddoc />
        public bool IsLast
        {
            get { return this.Next == null; }
        }

        /// <inheriteddoc />
        public virtual WorkflowActionNoState Next
        {
            get { return (WorkflowActionNoState)this._next; }

            set { this._next = value; }
        }

        /// <inheriteddoc />
        public IDictionary<string, object> NextVars
        {
            get { return this._nextVars; }

            set { this._nextVars = value; }
        }

        /// <inheriteddoc />
        public IReadOnlyDictionary<string, object> PreviousVars
        {
            get { return this._previousVars; }

            set { this._previousVars = value; }
        }

        /// <inheriteddoc />
        public object Result
        {
            get { return this._result; }

            set { this._result = value; }
        }

        /// <inheriteddoc />
        public object SyncRoot
        {
            get { return this._syncRoot; }

            set { this._syncRoot = value; }
        }

        /// <inheriteddoc />
        public bool ThrowErrors
        {
            get { return this._throwErrors; }

            set { this._throwErrors = value; }
        }

        /// <inheriteddoc />
        public IWorkflow Workflow
        {
            get { return this._workflow; }

            set { this._workflow = value; }
        }

        /// <inheriteddoc />
        public IDictionary<string, object> WorkflowVars
        {
            get { return this._workflowVars; }

            set { this._workflowVars = value; }
        }

        #endregion Properties

        #region Methods (22)

        // Public Methods (22) 

        /// <inheriteddoc />
        public T GetExecutionArgument<T>(int index)
        {
            T result;
            if (this.TryGetExecutionArgument<T>(index, out result) == false)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            return result;
        }

        /// <inheriteddoc />
        public T GetExecutionVar<T>(IEnumerable<char> name)
        {
            T result;
            if (this.TryGetExecutionVar<T>(name, out result) == false)
            {
                throw new InvalidOperationException();
            }

            return result;
        }

        /// <inheriteddoc />
        public T GetNextVar<T>(IEnumerable<char> name)
        {
            T result;
            if (this.TryGetNextVar<T>(name, out result) == false)
            {
                throw new InvalidOperationException();
            }

            return result;
        }

        /// <inheriteddoc />
        public T GetPreviousVar<T>(IEnumerable<char> name)
        {
            T result;
            if (this.TryGetPreviousVar<T>(name, out result) == false)
            {
                throw new InvalidOperationException();
            }

            return result;
        }

        /// <inheriteddoc />
        public T GetResult<T>()
        {
            return GlobalConverter.Current
                                  .ChangeType<T>(this.Result);
        }

        /// <inheriteddoc />
        public W GetWorkflow<W>() where W : global::MarcelJoachimKloubert.CLRToolbox.Execution.Workflows.IWorkflow
        {
            return GlobalConverter.Current
                                  .ChangeType<W>(this.Workflow);
        }

        /// <inheriteddoc />
        public T GetWorkflowVar<T>(IEnumerable<char> name)
        {
            T result;
            if (this.TryGetWorkflowVar<T>(name, out result) == false)
            {
                throw new InvalidOperationException();
            }

            return result;
        }

        /// <summary>
        /// Parses a char sequence to use it as variable name.
        /// </summary>
        /// <param name="name">The char sequence.</param>
        /// <returns>The parsed char sequence.</returns>
        public static string ParseVarName(IEnumerable<char> name)
        {
            return StringHelper.AsString(name) ?? string.Empty;
        }

        /// <inheriteddoc />
        public bool TryGetExecutionArgument<T>(int index, out T value)
        {
            return this.TryGetExecutionArgument<T>(index, out value, default(T));
        }

        /// <inheriteddoc />
        public bool TryGetExecutionArgument<T>(int index, out T value, T defaultValue)
        {
            return this.TryGetExecutionArgument<T>(index, out value,
                                                   delegate(int i)
                                                   {
                                                       return defaultValue;
                                                   });
        }

        /// <inheriteddoc />
        public bool TryGetExecutionArgument<T>(int index, out T value, Func<int, T> defaultValueProvider)
        {
            if (defaultValueProvider == null)
            {
                throw new ArgumentNullException("defaultValueProvider");
            }

            lock (this.SyncRoot)
            {
                IReadOnlyList<object> list = this.ExecutionArguments;
                if (list != null)
                {
                    if ((index > -1) && (index < list.Count))
                    {
                        value = GlobalConverter.Current
                                               .ChangeType<T>(list[index]);

                        return true;
                    }
                }
            }

            value = defaultValueProvider(index);
            return false;
        }

        /// <inheriteddoc />
        public bool TryGetExecutionVar<T>(IEnumerable<char> name, out T value)
        {
            return this.TryGetExecutionVar<T>(name, out value, default(T));
        }

        /// <inheriteddoc />
        public bool TryGetExecutionVar<T>(IEnumerable<char> name, out T value, T defaultValue)
        {
            return this.TryGetExecutionVar<T>(name, out value,
                                        delegate(string varName)
                                        {
                                            return defaultValue;
                                        });
        }

        /// <inheriteddoc />
        public bool TryGetExecutionVar<T>(IEnumerable<char> name, out T value, Func<string, T> defaultValueProvider)
        {
            if (defaultValueProvider == null)
            {
                throw new ArgumentNullException("defaultValueProvider");
            }

            lock (this.SyncRoot)
            {
                IDictionary<string, object> dict = this.ExecutionVars;
                if (dict != null)
                {
                    object foundValue;
                    if (dict.TryGetValue(ParseVarName(name), out foundValue))
                    {
                        value = GlobalConverter.Current
                                               .ChangeType<T>(foundValue);

                        return true;
                    }
                }
            }

            value = defaultValueProvider(StringHelper.AsString(name));
            return false;
        }

        /// <inheriteddoc />
        public bool TryGetNextVar<T>(IEnumerable<char> name, out T value)
        {
            return this.TryGetNextVar<T>(name, out value, default(T));
        }

        /// <inheriteddoc />
        public bool TryGetNextVar<T>(IEnumerable<char> name, out T value, T defaultValue)
        {
            return this.TryGetNextVar<T>(name, out value,
                                         delegate(string varName)
                                         {
                                             return defaultValue;
                                         });
        }

        /// <inheriteddoc />
        public bool TryGetNextVar<T>(IEnumerable<char> name, out T value, Func<string, T> defaultValueProvider)
        {
            if (defaultValueProvider == null)
            {
                throw new ArgumentNullException("defaultValueProvider");
            }

            lock (this.SyncRoot)
            {
                IDictionary<string, object> dict = this.NextVars;
                if (dict != null)
                {
                    object foundValue;
                    if (dict.TryGetValue(ParseVarName(name), out foundValue))
                    {
                        value = GlobalConverter.Current
                                               .ChangeType<T>(foundValue);

                        return true;
                    }
                }
            }

            value = defaultValueProvider(StringHelper.AsString(name));
            return false;
        }

        /// <inheriteddoc />
        public bool TryGetPreviousVar<T>(IEnumerable<char> name, out T value)
        {
            return this.TryGetPreviousVar<T>(name, out value, default(T));
        }

        /// <inheriteddoc />
        public bool TryGetPreviousVar<T>(IEnumerable<char> name, out T value, T defaultValue)
        {
            return this.TryGetPreviousVar<T>(name, out value,
                                             delegate(string varName)
                                             {
                                                 return defaultValue;
                                             });
        }

        /// <inheriteddoc />
        public bool TryGetPreviousVar<T>(IEnumerable<char> name, out T value, Func<string, T> defaultValueProvider)
        {
            if (defaultValueProvider == null)
            {
                throw new ArgumentNullException("defaultValueProvider");
            }

            lock (this.SyncRoot)
            {
                IReadOnlyDictionary<string, object> dict = this.PreviousVars;
                if (dict != null)
                {
                    object foundValue;
                    if (dict.TryGetValue(ParseVarName(name), out foundValue))
                    {
                        value = GlobalConverter.Current
                                               .ChangeType<T>(foundValue);

                        return true;
                    }
                }
            }

            value = defaultValueProvider(StringHelper.AsString(name));
            return false;
        }

        /// <inheriteddoc />
        public bool TryGetWorkflowVar<T>(IEnumerable<char> name, out T value)
        {
            return this.TryGetWorkflowVar<T>(name, out value, default(T));
        }

        /// <inheriteddoc />
        public bool TryGetWorkflowVar<T>(IEnumerable<char> name, out T value, T defaultValue)
        {
            return this.TryGetWorkflowVar<T>(name, out value,
                                             delegate(string varName)
                                             {
                                                 return defaultValue;
                                             });
        }

        /// <inheriteddoc />
        public bool TryGetWorkflowVar<T>(IEnumerable<char> name, out T value, Func<string, T> defaultValueProvider)
        {
            if (defaultValueProvider == null)
            {
                throw new ArgumentNullException("defaultValueProvider");
            }

            lock (this.Workflow.SyncRoot)
            {
                IDictionary<string, object> dict = this.WorkflowVars;
                if (dict != null)
                {
                    object foundValue;
                    if (dict.TryGetValue(ParseVarName(name), out foundValue))
                    {
                        value = GlobalConverter.Current
                                               .ChangeType<T>(foundValue);

                        return true;
                    }
                }
            }

            value = defaultValueProvider(StringHelper.AsString(name));
            return false;
        }

        #endregion Methods
    }

    #endregion

    #region CLASS: SimpleWorkflowExecutionContext<S>

    /// <summary>
    /// Simple implementation of <see cref="IWorkflowExecutionContext{S}" /> interface.
    /// </summary>
    /// <typeparam name="S">Type of the underlying state objects.</typeparam>
    public sealed class SimpleWorkflowExecutionContext<S> : SimpleWorkflowExecutionContext, IWorkflowExecutionContext<S>
    {
        #region Fields (2)

        private WorkflowAction<S> _nextWithState;
        private S _state;

        #endregion Fields

        #region Properties (4)

        /// <inheriteddoc />
        public override WorkflowActionNoState Next
        {
            get { return base.Next; }

            set
            {
                base.Next = value;
                this._nextWithState = value == null ? null : new WorkflowAction<S>(delegate(IWorkflowExecutionContext<S> ctx)
                    {
                        value(ctx);
                    });
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="IWorkflowExecutionContext{S}.Next" />
        public WorkflowAction<S> NextWithState
        {
            get { return this._nextWithState; }

            set
            {
                this._nextWithState = value;
                this.Next = value == null ? null : new WorkflowActionNoState(delegate(IWorkflowExecutionContext ctx)
                    {
                        value((IWorkflowExecutionContext<S>)ctx);
                    });
            }
        }

        /// <inheriteddoc />
        public S State
        {
            get { return this._state; }

            set { this._state = value; }
        }

        WorkflowAction<S> IWorkflowExecutionContext<S>.Next
        {
            get { return this.NextWithState; }

            set { this.NextWithState = value; }
        }

        #endregion Properties
    }

    #endregion
}