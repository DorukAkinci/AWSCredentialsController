using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDesk.Options;
using ConsoleTables;
using Console = Colorful.Console;
using System.Drawing;

using Amazon;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Amazon.Runtime.CredentialManagement;
using Amazon.Runtime;

namespace AWSCredentialsController
{
    class Program
    {
        static string _newProfile = "";
        static string _profile = "";
        static string _accessKey = "";
        static string _secretKey = "";
        static string _roleARN = "";
        static string _sessionName = "AWSCredentialsController";
        static bool _help = false;
        static bool _list = false;
        static SharedCredentialsFile _sharedCredentialsFile;
        static AmazonSecurityTokenServiceClient _stsClient;

        static void Main(string[] args)
        {
            try
            {
                OptionSet _optionSet = new OptionSet(){
                    { "l|list", "Active profilleri listelemeyi sağlar." , q => _list = q != null},
                    { "np|newprofile=", "Yeni credential tanımı için profil adı belirlemenizi sağlar.", q => _newProfile = q },
                    { "ra|rolearn=", "Cross Role yapılacak IAM Role ARN", q => _roleARN = q },
                    { "sn|sessionname=", "STS Assume Role sırasında atanacak session name olarak kullanılır. ( Default: AWSCredentialsController )", q => _sessionName = q },

                    { "p|profile=", "Amazon ortamına bağlanmak için hazırda profile tanımı varsa kullanabilmeyi sağlar.", q => _profile = q },
                    { "ak|accesskey=", "Amazon ortamına bağlanmak için kullanılacak access key'i tanımlar.", q => _accessKey = q },
                    { "sk|secretkey=", "Amazon ortamına bağlanmak için kullanılacak secret key'i tanımlar.", q => _secretKey = q },
                    { "h|?|help", "Bilgi almak için kullanılabilir." , q => _help = q != null}
                };

                var container = _optionSet.Parse(args);
                if ((_help) || (args.Length == 0))
                {
                    Console.WriteLine("Program için kullanılabilinecek komutlar:");
                    _optionSet.WriteOptionDescriptions(Console.Out);
                }
                else
                    ExecuteTheParameters();

                Console.WriteLine("The program has been successfully completed.", Color.Green);
            }
            catch (OptionException ex)
            {
                Console.WriteLine(ex.Message, Color.Red);
                Console.WriteLine("You can use --help for instructions", Color.Red);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected ERROR: " + ex.Message, Color.Red);
            }
#if DEBUG
            finally
            {
                Console.ReadKey();
            }
#endif
        }

        private static void ExecuteTheParameters()
        {
            _sharedCredentialsFile = new SharedCredentialsFile();
            if (((_profile != "") && (_roleARN != "")) || (((_accessKey != "") && (_secretKey != "") && (_roleARN != "")))) CreateSessionProfile();
            if (_list) ListCredentialProfiles();
        }

        private static void CreateSessionProfile()
        {
            Console.WriteLine("New Profile " + _newProfile + " is registering for RoleARN " + _roleARN, Color.Aqua);
            CredentialProfile _credentialProfileForSTSClient;
            if (_profile != "")
                _credentialProfileForSTSClient = GetAWSCredentialProfile(_profile);
            else
                _credentialProfileForSTSClient = new CredentialProfile("CredentialController", new CredentialProfileOptions { AccessKey = _accessKey, SecretKey = _secretKey });

            InitializeSTSClient(_credentialProfileForSTSClient);
            var _assumedRoleCredentials = AssumeRoleWithSTS(_roleARN, _sessionName);
            RegisterAWSProfile(_newProfile, _assumedRoleCredentials);
        }

        private static void RegisterAWSProfile(string ProfileName, Credentials RoleCredentials)
        {
            _sharedCredentialsFile.RegisterProfile(new CredentialProfile(ProfileName, new CredentialProfileOptions { AccessKey = RoleCredentials.AccessKeyId, SecretKey = RoleCredentials.SecretAccessKey, Token = RoleCredentials.SessionToken }));
        }

        private static Credentials AssumeRoleWithSTS(string _roleARN, string _sessionName)
        {
            return _stsClient.AssumeRole(new AssumeRoleRequest { RoleArn = _roleARN, RoleSessionName = _sessionName }).Credentials;
        }

        private static void InitializeSTSClient(CredentialProfile _credentialProfile)
        {
            _stsClient = new AmazonSecurityTokenServiceClient(_credentialProfile.Options.AccessKey, _credentialProfile.Options.SecretKey, RegionEndpoint.USEast1);
        }

        private static CredentialProfile GetAWSCredentialProfile(string ProfileName)
        {
            CredentialProfile _credentialProfile;
            if (_sharedCredentialsFile.TryGetProfile(ProfileName, out _credentialProfile))
            {
                return _credentialProfile;
            }
            else
                throw new Exception("Belirtilen Profile bulunamadı.");
        }

        private static void ListCredentialProfiles()
        {
            ConsoleOutputMakeStars();
            Console.WriteLine("Listing all existing profiles:", Color.Aqua);
            var _table = new ConsoleTable("Profile", "Type", "AccessKey", "SecretKey");
            SharedCredentialsFile _sharedCredentialsFile = new SharedCredentialsFile();
            var _profileList = _sharedCredentialsFile.ListProfiles();
            foreach (var _profile in _profileList)
            {
                _table.AddRow(_profile.Name, _profile.CredentialDescription, _profile.Options.AccessKey, _profile.Options.SecretKey);
            }
            _table.Write();
        }

        private static void ConsoleOutputMakeStars()
        {
            Console.WriteLine("*******************************");
        }
    }
}
