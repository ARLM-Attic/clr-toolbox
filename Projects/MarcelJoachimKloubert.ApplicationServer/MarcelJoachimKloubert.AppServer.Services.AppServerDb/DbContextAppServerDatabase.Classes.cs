﻿// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. http://blog.marcel-kloubert.de


using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MarcelJoachimKloubert.CLRToolbox.Data;
using MarcelJoachimKloubert.CLRToolbox.Helpers;

namespace MarcelJoachimKloubert.AppServer.Services.AppServerDb
{
    partial class DbContextAppServerDatabase
    {
        #region Nested Classes (1)

        private class AppServerDbContext : DbContext
        {
            #region Fields (3)

            private readonly IDictionary<Type, object> _DB_SETS = new Dictionary<Type, object>();
            private readonly Assembly[] _ENTITY_ASSEMBLIES;
            private readonly object _SYNC = new object();

            #endregion Fields

            #region Constructors (1)

            internal AppServerDbContext(DbConnection conn,
                                        IEnumerable<Assembly> entityAssemblies,
                                        bool ownConnection)
                : base(conn, ownConnection)
            {
                this._ENTITY_ASSEMBLIES = (entityAssemblies ?? Enumerable.Empty<Assembly>()).OfType<Assembly>()
                                                                                            .Distinct()
                                                                                            .ToArray();

                this.InvokeForDbSetList(list =>
                                        {
                                            foreach (var et in this.GetEntityTypes())
                                            {
                                                // DbContext.Set<E>()
                                                var setMethod = this.GetType()
                                                                    .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                                                                    .Single(m => m.Name == "Set" &&
                                                                                 m.GetGenericArguments().Length == 1 &&
                                                                                 m.GetParameters().Length == 0)
                                                                    .MakeGenericMethod(et);

                                                list.Add(et,
                                                         setMethod.Invoke(this,
                                                                          new object[0]));
                                            }
                                        });
            }

            #endregion Constructors

            #region Methods (5)

            // Protected Methods (1) 

            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                foreach (var et in this.GetEntityTypes())
                {
                    var tableAttrib = et.GetCustomAttributes(typeof(global::MarcelJoachimKloubert.CLRToolbox.Data.TMTableAttribute), false)
                                        .Cast<TMTableAttribute>()
                                        .SingleOrDefault();

                    string tableName;
                    string schemaName = null;
                    if (tableAttrib != null)
                    {
                        tableName = tableAttrib.Name;
                        schemaName = tableAttrib.Schema;
                    }
                    else
                    {
                        tableName = et.Name;
                    }

                    // primary keys
                    var keyProperties = et.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                          .Where(p =>
                                                 {
                                                     return p.GetCustomAttributes(typeof(global::MarcelJoachimKloubert.CLRToolbox.Data.TMColumnAttribute), true)
                                                             .Cast<TMColumnAttribute>()
                                                             .Where(a => a.IsKey)
                                                             .SingleOrDefault() != null;
                                                 });

                    // not marked as scalar properties
                    var nonScalarProperties = et.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                                .Where(p =>
                                                       {
                                                           return p.GetCustomAttributes(typeof(global::MarcelJoachimKloubert.CLRToolbox.Data.TMColumnAttribute), true)
                                                                   .Cast<TMColumnAttribute>()
                                                                   .SingleOrDefault() == null;
                                                       });

                    // DbModelBuilder.Entity<E>()
                    var entityMethod = modelBuilder.GetType()
                                                   .GetMethod("Entity", BindingFlags.Instance | BindingFlags.Public)
                                                   .MakeGenericMethod(et);

                    var entityTypeConf = entityMethod.Invoke(modelBuilder, new object[0]);
                    if (entityTypeConf != null)
                    {
                        // EntityTypeConfiguration<TEntityType>.ToTable()

                        var entityTypeConfType = entityTypeConf.GetType();

                        object[] args;
                        if (string.IsNullOrWhiteSpace(schemaName))
                        {
                            args = new object[] { tableName };
                        }
                        else
                        {
                            // with schema

                            args = new object[] { tableName,
                                                  schemaName };
                        }

                        // and invoke...
                        CollectionHelper.Single(entityTypeConfType.GetMethods(BindingFlags.Instance | BindingFlags.Public),
                                                m => m.Name == "ToTable" &&
                                                     m.GetParameters().Length == args.Length)
                                        .Invoke(entityTypeConf,
                                                args);
                    }

                    // map primary keys
                    foreach (var key in keyProperties)
                    {
                        InvokeEntityTypeConfigMethodWithLambdaExpr(et,
                                                                   key,
                                                                   entityTypeConf,
                                                                   "HasKey");
                    }

                    // ignore all properties that are not marked as scalar properties
                    foreach (var prop in nonScalarProperties)
                    {
                        InvokeEntityTypeConfigMethodWithLambdaExpr(et,
                                                                   prop,
                                                                   entityTypeConf,
                                                                   "Ignore");
                    }
                }
            }
            // Private Methods (3) 

            private IEnumerable<Type> GetEntityTypes()
            {
                return this._ENTITY_ASSEMBLIES
                           .SelectMany(asm => asm.GetTypes())
                           .Where(t => t.IsClass &&
                                       !t.IsAbstract && t.GetInterfaces()
                                                         .Contains(typeof(global::MarcelJoachimKloubert.ApplicationServer.Data.Entities.IAppServerEntity)));
            }

            private static void InvokeEntityTypeConfigMethodWithLambdaExpr(Type entityType,
                                                                           PropertyInfo entityProperty,
                                                                           object entityTypeConfiguration,
                                                                           string methodName)
            {
                var propertyType = entityProperty.PropertyType;

                var method = entityTypeConfiguration.GetType()
                                                    .GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public)
                                                    .MakeGenericMethod(propertyType);

                Expression methodParam;
                {
                    var funcType = Expression.GetFuncType(entityType, propertyType);
                    var funcParam = Expression.Parameter(entityType, "e");

                    var propertyExpr = Expression.Property(funcParam,
                                                           entityProperty);

                    // Expression.Lambda(Expression, ParameterExpression[])
                    var lambdaMethod = typeof(global::System.Linq.Expressions.Expression).GetMethods(BindingFlags.Static | BindingFlags.Public)
                                                                                         .Single(m => m.Name == "Lambda" &&
                                                                                                      m.GetGenericArguments().Length == 1 &&
                                                                                                      m.GetParameters().Length == 2 &&
                                                                                                      m.GetParameters()[0].ParameterType.Equals(typeof(global::System.Linq.Expressions.Expression)) &&
                                                                                                      m.GetParameters()[1].ParameterType.Equals(typeof(global::System.Linq.Expressions.ParameterExpression[])))
                                                                                         .MakeGenericMethod(funcType);

                    methodParam = (Expression)lambdaMethod.Invoke(null,
                                                                  new object[]
                                                                  {
                                                                      propertyExpr,
                                                                      new ParameterExpression[] { funcParam, },
                                                                  });
                }

                method.Invoke(entityTypeConfiguration,
                              new object[]
                              {
                                  methodParam,
                              });
            }

            private void InvokeForDbSetList(Action<IDictionary<Type, object>> action)
            {
                lock (this._SYNC)
                {
                    action(this._DB_SETS);
                }
            }
            // Internal Methods (1) 

            internal DbSet<E> GetDbSet<E>() where E : class, global::MarcelJoachimKloubert.ApplicationServer.Data.Entities.IAppServerEntity
            {
                var entityType = typeof(E);

                object result = null;
                this.InvokeForDbSetList(list =>
                                        {
                                            result = list[entityType];
                                        });

                return (DbSet<E>)result;
            }

            #endregion Methods
        }

        #endregion Nested Classes
    }
}