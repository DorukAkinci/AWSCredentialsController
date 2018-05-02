using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDesk.Options;
using ConsoleTables;
using Console = Colorful.Console;
using System.Drawing;
using Amazon.Runtime.CredentialManagement;

namespace AWSCredentialsController
{
    class Program
    {
        static string _newProfile = "";
        static string _profile = "";
        static string _accessKey = "";
        static string _secretKey = "";
        static bool _help = false;
        static bool _list = false;

        static void Main(string[] args)
        {
            try
            {

                OptionSet _optionSet = new OptionSet(){
                    { "l|list", "Active profilleri listelemeyi sağlar." , q => _list = q != null},
                    { "np|newprofile=", "Yeni credential tanımı için profil adı belirlemenizi sağlar.", q => _newProfile = q },
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

                Console.WriteLine("Program tamamlandı.");
            }
            catch (OptionException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("You can use --help for instructions");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected ERROR: " + ex.Message);
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
            if (_list) ListCredentialProfiles();
        }

        private static void ListCredentialProfiles()
        {
            ConsoleOutputMakeStars();
            Console.WriteLine("Listing all existing profiles:", Color.Red);
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
            Console.WriteLine("******************");
        }
    }
}
