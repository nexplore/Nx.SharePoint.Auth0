using System.Collections.Generic;
using Microsoft.SharePoint.Administration;

namespace Nx.SharePoint.Auth0.Helpers
{
    public class LoggingService : SPDiagnosticsServiceBase
    {
        public static string DiagnosticAreaName = "Nexplore";
        public static string ErrorCategoryName = "Auth0SyncError";
        public static string InfoCategoryName = "Auth0SyncInfo";
        private static LoggingService _current;

        public static LoggingService Current
        {
            get { return _current ?? (_current = new LoggingService()); }
        }

        private LoggingService()
            : base("Nexplore Logging Service", SPFarm.Local)
        {
        }

        protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
        {
            var areas = new List<SPDiagnosticsArea>
            {
                new SPDiagnosticsArea(DiagnosticAreaName, new List<SPDiagnosticsCategory>
                {
                    new SPDiagnosticsCategory(ErrorCategoryName, TraceSeverity.Unexpected, EventSeverity.Error),
                    new SPDiagnosticsCategory(InfoCategoryName, TraceSeverity.Verbose, EventSeverity.Information)
                })
            };

            return areas;
        }

        public static void LogError(string errorMessage)
        {
            var category = Current.Areas[DiagnosticAreaName].Categories[ErrorCategoryName];
            Current.WriteTrace(0, category, TraceSeverity.Unexpected, errorMessage);
        }

        public static void LogInformation(string message)
        {
            var category = Current.Areas[DiagnosticAreaName].Categories[InfoCategoryName];
            Current.WriteTrace(0, category, TraceSeverity.Verbose, message);
        }
    }

}
