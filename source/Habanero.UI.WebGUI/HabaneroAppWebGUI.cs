//---------------------------------------------------------------------------------
// Copyright (C) 2008 Chillisoft Solutions
// 
// This file is part of the Habanero framework.
// 
//     Habanero is a free framework: you can redistribute it and/or modify
//     it under the terms of the GNU Lesser General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     The Habanero framework is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU Lesser General Public License for more details.
// 
//     You should have received a copy of the GNU Lesser General Public License
//     along with the Habanero framework.  If not, see <http://www.gnu.org/licenses/>.
//---------------------------------------------------------------------------------

using Habanero.BO;
using Habanero.UI.Base;
using Habanero.DB;
using Habanero.Base;

namespace Habanero.UI.WebGUI
{
    public class HabaneroAppWebGUI : HabaneroAppUI
    {
                /// <summary>
        /// Constructor to initialise a new application with basic application
        /// information.  Use the Startup() method to launch the application.
        /// </summary>
        /// <param name="appName">The application name</param>
        /// <param name="appVersion">The application version</param>
        public HabaneroAppWebGUI(string appName, string appVersion)
            : base(appName, appVersion)
        {
            SetupControlFactory();

        }

        protected override void SetupControlFactory()
        {
            GlobalUIRegistry.ControlFactory = new ControlFactoryGizmox();
        }

        /// <summary>
        /// Initialises the settings.  If not provided, DatabaseSettings
        /// is assumed.
        /// </summary>
        protected override void SetupSettings()
        {
            if (Settings == null) Settings = new DatabaseSettings();
            GlobalRegistry.Settings = Settings;
        }

        /// <summary>
        /// Sets up the database connection.  If not provided, then
        /// reads the connection from the config file.
        /// </summary>
        protected override void SetupDatabaseConnection()
        {
            if (DatabaseConnection.CurrentConnection != null) return;
            if (_databaseConfig == null) _databaseConfig = DatabaseConfig.ReadFromConfigFile();
            if (_privateKey != null) _databaseConfig.SetPrivateKey(_privateKey);
            DatabaseConnection.CurrentConnection = _databaseConfig.GetDatabaseConnection();
            BORegistry.DataAccessor = new DataAccessorDB();
        }


        /// <summary>
        /// Sets the database configuration object, which contains basic 
        /// connection information along with the database vendor name 
        /// (eg. MySql, Oracle).
        /// </summary>
        public DatabaseConfig DatabaseConfig
        {
            set { _databaseConfig = value; }
        }

        private DatabaseConfig _databaseConfig;

        /// <summary>
        /// Sets up the exception notifier used to display
        /// exceptions to the final user.  If not specified,
        /// assumes the FormExceptionNotifier.
        /// </summary>
        protected override void SetupExceptionNotifier()
        {
            if (ExceptionNotifier == null) ExceptionNotifier = new FormExceptionNotifier();
            GlobalRegistry.UIExceptionNotifier = ExceptionNotifier;
        }
    }
}
