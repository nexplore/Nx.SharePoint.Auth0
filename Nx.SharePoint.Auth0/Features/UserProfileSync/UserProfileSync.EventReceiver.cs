using System;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Nx.SharePoint.Auth0.TimerJobs;

namespace Nx.SharePoint.Auth0.Features.UserProfileSync
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>
    [Guid("1fce3706-b719-4572-87f6-f3c4023fb652")]
    public class UserProfileSyncEventReceiver : SPFeatureReceiver
    {
        private const string JobName = "Auth0UserProfileSyncJob";

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                var parentWebApp = (SPWebApplication) properties.Feature.Parent;
                DeleteExistingJob(JobName, parentWebApp);
                CreateJob(parentWebApp);
            });
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            lock (this)
            {
                SPSecurity.RunWithElevatedPrivileges(() =>
                {
                    var parentWebApp = (SPWebApplication) properties.Feature.Parent;
                    DeleteExistingJob(JobName, parentWebApp);
                });
            }
        }

        private void CreateJob(SPWebApplication webApplication)
        {
            var job = new UserProfileSyncTimerJob(JobName, webApplication);
            var schedule = new SPWeeklySchedule
            {
                BeginDayOfWeek = DayOfWeek.Sunday,
                BeginHour = 3,
                BeginMinute = 0,
                BeginSecond = 0,
                EndDayOfWeek = DayOfWeek.Sunday,
                EndHour = 5,
                EndMinute = 59,
                EndSecond = 59
            };

            job.Schedule = schedule;

            job.Update();
        }

        public void DeleteExistingJob(string jobName, SPWebApplication webApplication)
        {
            foreach (var job in webApplication.JobDefinitions.Where(job => job.Name == jobName))
            {
                job.Delete();
            }
        }
    }
}
