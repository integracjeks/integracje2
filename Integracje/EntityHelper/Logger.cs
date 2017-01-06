using System;
using System.Linq;

namespace EntityHelper
{
    public class Logger
    {
        #region Constructors

        public Logger(string info, DateTime dateTime, string name)
        {
            this.info = info;
            this.dateTime = dateTime;
            this.name = name;

            Log();
        }

        #endregion Constructors

        #region Methods

        public int GetActivitiesCountOfLastTenMinutes()
        {
            using (var db = new DbLogsContext())
            {
                try
                {
                    var list = db.DbLogs.ToList();
                    var logCountList = from log in list
                                       where (log.data + TimeSpan.FromMinutes(1)).CompareTo(dateTime) == 1 && log.ip.Trim() == info
                                       select log;

                    return logCountList.Count();
                }
                catch
                {
                    return -1;
                }
            }
        }

        #endregion Methods

        #region Fields

        private DateTime dateTime;
        private string info;
        private string name;

        #endregion Fields

        private void Log()
        {
            using (var db = new DbLogsContext())
            {
                try
                {
                    var log = db.CreateDefaultLog();
                    log.data = dateTime;
                    log.procedure_name = name;
                    log.ip = info;

                    db.DbLogs.Add(log);
                    db.SaveChanges();
                }
                catch { }
            }
        }
    }
}