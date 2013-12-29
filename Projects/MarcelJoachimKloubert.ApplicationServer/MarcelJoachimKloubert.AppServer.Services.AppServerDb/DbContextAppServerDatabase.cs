﻿// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. http://blog.marcel-kloubert.de


using System;
using System.ComponentModel.Composition;
using System.Linq;
using MarcelJoachimKloubert.ApplicationServer.DataLayer;
using AppServerImpl = MarcelJoachimKloubert.ApplicationServer.ApplicationServer;

namespace MarcelJoachimKloubert.AppServer.Services.AppServerDb
{
    [Export(typeof(global::MarcelJoachimKloubert.ApplicationServer.DataLayer.IAppServerDatabase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal sealed partial class DbContextAppServerDatabase : AppServerDatabaseBase
    {
        #region Fields (2)

        private readonly AppServerDbContext _DB_CONTEXT;
        private readonly AppServerImpl _SERVER;

        #endregion Fields

        #region Constructors (1)

        [ImportingConstructor]
        internal DbContextAppServerDatabase(AppServerImpl server)
        {
            this._SERVER = server;

            // define DbConnection
            string dbProvider;
            var dbConn = this._SERVER.TryGetServerDbConnectionByConfig(out dbProvider);

            if (dbConn == null)
            {
                throw new NotSupportedException(string.Format("'{0}' provider is NOT supported!",
                                                              dbProvider));
            }

            this._DB_CONTEXT = new AppServerDbContext(dbConn,
                                                      this._SERVER.EntityAssemblies,
                                                      true);
        }

        #endregion Constructors

        #region Properties (1)

        public override bool CanUpdate
        {
            get { return true; }
        }

        #endregion Properties

        #region Methods (6)

        // Public Methods (4) 

        public override void Add<E>(E entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            this.ThrowIfDisposed();

            this._DB_CONTEXT
                .GetDbSet<E>()
                .Add(entity);
        }

        public override void Attach<E>(E entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            this.ThrowIfDisposed();

            this._DB_CONTEXT
                .GetDbSet<E>()
                .Attach(entity);
        }

        public override IQueryable<E> Query<E>()
        {
            return this._DB_CONTEXT
                       .GetDbSet<E>();
        }

        public override void Remove<E>(E entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            this.ThrowIfDisposed();

            this._DB_CONTEXT
                .GetDbSet<E>()
                .Remove(entity);
        }
        // Protected Methods (2) 

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                this._DB_CONTEXT
                    .Dispose();
            }
        }

        protected override void OnUpdate()
        {
            this._DB_CONTEXT.SaveChanges();
        }

        #endregion Methods
    }
}