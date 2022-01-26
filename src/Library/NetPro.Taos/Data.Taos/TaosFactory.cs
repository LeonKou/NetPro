// Copyright (c)  maikebing All rights reserved.
//// Licensed under the MIT License, See License.txt in the project root for license information.

using System.Data.Common;

namespace Maikebing.Data.Taos
{
    /// <summary>
    ///     Creates instances of various Maikebing.Data.Taos classes.
    /// </summary>
    public class TaosFactory : DbProviderFactory
    {
        private TaosFactory()
        {
        }

        /// <summary>
        ///     The singleton instance.
        /// </summary>
        public static readonly TaosFactory Instance = new TaosFactory();

        /// <summary>
        ///     Creates a new command.
        /// </summary>
        /// <returns>The new command.</returns>
        public override DbCommand CreateCommand()
            => new TaosCommand();

        /// <summary>
        ///     Creates a new connection.
        /// </summary>
        /// <returns>The new connection.</returns>
        public override DbConnection CreateConnection()
            => new TaosConnection();

        /// <summary>
        ///     Creates a new connection string builder.
        /// </summary>
        /// <returns>The new connection string builder.</returns>
        public override DbConnectionStringBuilder CreateConnectionStringBuilder()
            => new TaosConnectionStringBuilder();

        /// <summary>
        ///     Creates a new parameter.
        /// </summary>
        /// <returns>The new parameter.</returns>
        public override DbParameter CreateParameter()
            => new TaosParameter();
    }
}
