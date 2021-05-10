using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace probaSVS
{


    class Program

    {
     


        static byte[] IOFile()
        {

            using (FileStream fstream = File.OpenRead("C:/Users/novoseltsev.aleksey/Desktop/674dfaf3-e9a1-4f52-833a-fe36101b43b3/674dfaf3-e9a1-4f52-833a-fe36101b43b3.xml"))
            { // преобразуем документ в байты
                byte[] document = new byte[fstream.Length];
                // считываем данные
                fstream.Read(document, 0, document.Length);
               // fstream.Close();
                Console.WriteLine("файл считан успешно");
                return document;
            }
        }
        static byte[] IOSign()
        {

            using (FileStream fstream1 = File.OpenRead("C:/Users/novoseltsev.aleksey/Desktop/674dfaf3-e9a1-4f52-833a-fe36101b43b3/674dfaf3-e9a1-4f52-833a-fe36101b43b3.xml.sig"))
            {  // преобразуем строку в байты
                byte[] signature = new byte[fstream1.Length];
                // считываем данные
                fstream1.Read(signature, 0, signature.Length);
                //fstream1.Close();
                Console.WriteLine("подпись считана успешно");
                return signature;
            }
        }




        static Task<svs.VerificationResult> QuerySVS(byte[] document, byte[] signature)
        {


            svs.VerificationServiceClient zapros = new svs.VerificationServiceClient(svs.VerificationServiceClient.EndpointConfiguration.BasicHttpBinding_IVerificationService);

            Dictionary<svs.VerifyParams, string> vp = new Dictionary<svs.VerifyParams, string>();
            vp.Add(svs.VerifyParams.SignatureIndex, "1");                                           //создание словаря для отправки

            return zapros.VerifyDetachedSignatureAsync(svs.SignatureType.CAdES, document, signature, vp); //выполнение запроса(тип подписи, документ, подпись, словарь)

        }

        static  void Main()
        {
            byte[] signature, document;
            document = IOFile();
            signature = IOSign();

            Console.WriteLine("------------------------");
 
            Console.WriteLine("Ожидаем ответ от сервера....");

        


            svs.VerificationResult otvetSVS = QuerySVS(document, signature).Result; //создаем объект класса svs.VerificationResult, вызываем метод запроса к серверу и записываем в объект ответ от сервиса
            
            Console.WriteLine($"{DateTime.Now} Result- {otvetSVS.Result}");// читаем результат task ...result от svs внутри result от task      
            
            if (otvetSVS.Message != null)
            {
                Console.WriteLine($"{DateTime.Now} Message- {otvetSVS.Message}");
                Console.WriteLine("------------------------");
            }


            Dictionary<svs.CertificateInfoParams, string> cp = otvetSVS.SignerCertificateInfo;    //запись ответа в словарь. т.к. инфа о серте пришла от сервиса в словаре

            Console.WriteLine($"Запрос выполнен - {cp[svs.CertificateInfoParams.SubjectName]}"); //чтение из словаря по ключу

            string zap = ",";
            string kav = "\"";
            Console.WriteLine("------------------------");
            Console.WriteLine($"Издатель - {cp[svs.CertificateInfoParams.IssuerName]}");




            string otvet = cp[svs.CertificateInfoParams.SubjectName] + ","; //добавляем запятую в конец чтобы искался последний элемент в строке
            Console.WriteLine("----------------------------------------");

            string s = "CN=";
            string CN = otvet.Substring(otvet.IndexOf(s) + s.Length, otvet.IndexOf(zap, otvet.IndexOf(s)) - (otvet.IndexOf(s) + s.Length));
            Console.WriteLine($"Организация - {CN}");


            s = "СНИЛС=";
            string snils = otvet.Substring(otvet.IndexOf(s) + s.Length, otvet.IndexOf(zap, otvet.IndexOf(s)) - (otvet.IndexOf(s) + s.Length));
            Console.WriteLine($"СНИЛC - {snils}");

           
            
                s = "ОГРН=";
            if (otvet.IndexOf(s) != -1)
            {
                string ogrn = otvet.Substring(otvet.IndexOf(s) + s.Length, otvet.IndexOf(zap, otvet.IndexOf(s)) - (otvet.IndexOf(s) + s.Length));
                Console.WriteLine($"ОГРН - {ogrn}");
            }
            else
            {
                {
                    ConsoleColor color = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Поле ОГРН отсутствует в сертификате");
                    Console.ForegroundColor = color;
                }

            }

            s = "ИНН=";
            string inn = otvet.Substring(otvet.IndexOf(s) + s.Length, otvet.IndexOf(zap, otvet.IndexOf(s)) - (otvet.IndexOf(s) + s.Length));
            Console.WriteLine($"ИНН - {Convert.ToInt64(inn)}");//конвертирую в int чтобы убрать нули в начале


            s = "SN=";
            string SN = otvet.Substring(otvet.IndexOf(s) + s.Length, otvet.IndexOf(zap, otvet.IndexOf(s)) - (otvet.IndexOf(s) + s.Length));
            Console.WriteLine($"Фамилия - {SN}");


            s = "G=";
            string G = otvet.Substring(otvet.IndexOf(s) + s.Length, otvet.IndexOf(zap, otvet.IndexOf(s)) - (otvet.IndexOf(s) + s.Length));
            Console.WriteLine($"Имя Отчество - {G}");


            s = "E=";
            if (otvet.IndexOf(s) != -1)
            {
                string mail = otvet.Substring(otvet.IndexOf(s) + s.Length, otvet.IndexOf(zap, otvet.IndexOf(s)) - (otvet.IndexOf(s) + s.Length));
                Console.WriteLine($"E-mail - {mail}");
            }
            else
            {
                ConsoleColor color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Поле E-mail отсутствует в сертификате");
                Console.ForegroundColor = color;
            }

           



            Console.ReadLine();
        }
    }
}
