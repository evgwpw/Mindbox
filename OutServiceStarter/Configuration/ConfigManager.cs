using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Inec.StateMachine.OutServiceStarter.Configuration
{
    public class ConfigManager
    {
        static string _packageName;
        /// <summary>
        /// имя пакета в конфигурационном файле
        /// </summary>
        public static string PackageName
        {
            get
            {
                if (string.IsNullOrEmpty(_packageName))
                {
                    _packageName = ConfigurationManager.AppSettings["PackageName"];
                    if (string.IsNullOrEmpty(_packageName))
                    {
                        throw new ConfigurationErrorsException("Не найден параметр PackageName в конфигурационном файле");
                    }                    
                }
                return _packageName;
            }
        }
        static string _connectionString;
        /// <summary>
        /// строка подключения к БД
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    _connectionString = ConfigurationManager.ConnectionStrings["SPDConnectionString"].ConnectionString;
                    if (string.IsNullOrEmpty(_connectionString))
                        throw new ConfigurationErrorsException("Не найдена строка подключения к БД в конфигурационном файле");
                }
                return _connectionString;
            }
        }
        static bool? _isTest;
        /// <summary>
        /// тестовая БД или бой
        /// </summary>
        public static bool IsTest
        {
            get
            {
                if (!_isTest.HasValue)
                {
                    var tmp = ConfigurationManager.AppSettings["IsTest"];
                    if(string.IsNullOrEmpty(tmp))
                        throw new ConfigurationErrorsException("Не найден параметр IsTest в конфигурационном файле");
                    _isTest = string.Compare(tmp, "1", StringComparison.InvariantCultureIgnoreCase) == 0;
                }
                return _isTest.Value;
            }
        }
        /// <summary>
        /// Название организации заявителя
        /// </summary>
        public static string NameApplicantOrganization
        {
            get
            {
                return ConfigurationManager.AppSettings["NameApplicantOrganization"];
            }
        }
        /// <summary>
        /// Фамилия И.О. заявителя
        /// </summary>
        public static string ApplicantFIO
        {
            get
            {
                return ConfigurationManager.AppSettings["ApplicantFIO"];
            }
        }
        /// <summary>
        /// комментарий
        /// </summary>
        public static string Comment
        {
            get
            {
                return ConfigurationManager.AppSettings["Comment"];
            }
        }
        /// <summary>
        /// Пометки
        /// </summary>
        public static string Markups
        {
            get
            {
                return ConfigurationManager.AppSettings["Markups"];
            }
        }
        /// <summary>
        /// почтовый адрес
        /// </summary>
        public static decimal  ZipCode
        {
            get
            {
                var tmp = ConfigurationManager.AppSettings["ZipCode"];
                decimal d;
                if (!decimal.TryParse(tmp, out d))
                {
                    return 0;
                }
                return d;
            }
        }
        /// <summary>
        /// почтовый адрес
        /// </summary>
        public static string MailingAddress
        {
            get
            {
                return ConfigurationManager.AppSettings["MailingAddress"];
            }
        }
        public static string Phone
        {
            get
            {
                return ConfigurationManager.AppSettings["Phone"];
            }
        }
        public static string Email
        {
            get
            {
                return ConfigurationManager.AppSettings["Email"];
            }
        }
        public static string ServiceUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["ServiceUrl"];
            }
        }

        public static string Rublicator
        {
            get
            {
                return ConfigurationManager.AppSettings["Rublicator"];
            }
        }

        public static string Fls
        {
            get
            {
                return ConfigurationManager.AppSettings["Fls"];
            }
        }
    }
}
