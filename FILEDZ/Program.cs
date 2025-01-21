using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;

namespace FILEDZ
{
    internal class Program
    {

        static string path1 = "c:\\Otus\\TestDir1";
        static string path2 = "c:\\Otus\\TestDir2";

        static void Main(string[] args)
        {
            CreateDirectory(path1);
            CreateDirectory(path2);
            CreateFiles(path1);
            CreateFiles(path2);
            ReadFiles(path1);
            ReadFiles(path2);

        }
        private static void CreateFiles(string path)
        {
            for (int i = 0; i < 10; i++)
            {
                string filePath = Path.Combine(path, $"Otus{i + 1}.txt");

                File.Create(filePath).Close();

                if (HasWriteAccess(filePath))
                {
                    using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
                    {
                        sw.WriteLine($"Otus{i + 1}");
                        sw.WriteLine(DateTime.Now);
                    }
                }
                else
                {
                    Console.WriteLine($"Нет прав на запись в файл: {filePath}");
                }
            }
            
        }
        public static void CreateDirectory(string path)
        {
            var dir = new DirectoryInfo(path);
            dir.Create();
        }
        private static void ReadFiles(string path)
        {
            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                try
                {
                    using (StreamReader sr = new StreamReader(file, Encoding.UTF8))
                    {
                        string text = sr.ReadToEnd();
                        string name = Path.GetFileName(file);
                        Console.WriteLine($"{name} : {text}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при чтении файла {file}: {ex.Message}");
                }
            }
        }
        //взято из примера с урока.
        static bool HasWriteAccess(string filePath)
        {
            return HasAccess(filePath, FileSystemRights.WriteData);
        }
        
        static bool HasAccess(string filePath, FileSystemRights accessRight)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                FileSecurity fileSecurity = fileInfo.GetAccessControl();
                AuthorizationRuleCollection rules = fileSecurity.GetAccessRules(true, true, typeof(NTAccount));

                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);

                foreach (FileSystemAccessRule rule in rules)
                {
                    if (principal.IsInRole(rule.IdentityReference.Value))
                    {
                        if ((rule.FileSystemRights & accessRight) == accessRight)
                        {
                            if (rule.AccessControlType == AccessControlType.Allow)
                            {
                                return true; 
                            }
                        }
                    }
                }
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false; 


            }
        }

    }
}
