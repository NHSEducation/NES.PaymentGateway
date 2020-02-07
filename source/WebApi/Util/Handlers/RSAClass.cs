using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PaymentGateway.Util.Handlers
{
    public class RSAClass
    {
        private static string _privateKey = "<RSAKeyValue><Modulus>0PM8rw9vsLQsw4sqzv7lSekl25yUxy07ChgD8ThbgMNcQ8fYICksPFY+yDPcWgkqJfbqQmJ5NkEToGXkMHqvN9KTqxD94IR+C6likRZhCmWP1/11C1jqwt4AyzRow6DBL774D6wliDt9zzgHR74ta1Mo+H6VgANFvwZfKTlrGkk=</Modulus><Exponent>AQAB</Exponent><P>6d9PrPtd5TebaTB6utNvg45HOU8/GKgPKketmBWv0wzQf0eru3sQRJHJM2/qE/dyTW4IWZrSvhD+NCogdmAEMQ==</P><Q>5LhHY2NdUHqf9OjVJMNJIJGGB1ET81/kHPh1264+k+N3LhMulurlopW7zWONwdnELg3KmpJHZ75PZoZtg7jpmQ==</Q><DP>5wggKG0E25vsHaSziP236ojR2U5ssUnL+WNnnJbH40CEc0f04Tb34hT2YqbK4UCPnOf2vbXRO8uDozp+aRH+8Q==</DP><DQ>iMDKJlnWBHWdSGEid/2vrqJ0IdHPfPf7u3qvdW9EiUY7Dzh25dODA+hFRvPcYWikTOkAF9WYjiYS8Xk058pbGQ==</DQ><InverseQ>wccBqWVVa2San3gpvE+RwKApwbQYJd10c5cDhC53TDALv1S3vZ680eFdVsD44nJurMWS0uXH4HHnMB5BFX8D7w==</InverseQ><D>wxMjh4iQzNcZp6II4GHmrP9HQeXInL9kjELpg04LV4aMYBd2dmgaDWazjHOmeERwHiuMf1eyNG3DJg+aX4xagcJsmDwQWGNdMuTmykSd1IAU22F582qMoiPrT3IyMYOdvtua+tEEYlfsi7NH4wXa99aXpyG138xHpRJUXEpcrIE=</D></RSAKeyValue>";
        private static string _publicKey = "<RSAKeyValue><Modulus>0PM8rw9vsLQsw4sqzv7lSekl25yUxy07ChgD8ThbgMNcQ8fYICksPFY+yDPcWgkqJfbqQmJ5NkEToGXkMHqvN9KTqxD94IR+C6likRZhCmWP1/11C1jqwt4AyzRow6DBL774D6wliDt9zzgHR74ta1Mo+H6VgANFvwZfKTlrGkk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        private static UnicodeEncoding _encoder = new UnicodeEncoding();

        public static string Decrypt(string data)
        {

            try
            {
                var rsa = new RSACryptoServiceProvider();

                var dataArray = data.Split(new[] { ',' });
                var dataByte = new byte[dataArray.Length];

                for (var i = 0; i < dataArray.Length; i++)
                {
                    dataByte[i] = Convert.ToByte(dataArray[i]);
                }

                rsa.FromXmlString(_privateKey);
                var decryptedByte = rsa.Decrypt(dataByte, false);

                return _encoder.GetString(decryptedByte);
            }
            catch (Exception)
            {
                throw new RSAException();
            }
        }

        public static string Encrypt(string data)
        {
            try
            {
                var rsa = new RSACryptoServiceProvider();

                rsa.FromXmlString(_publicKey);
                var dataToEncrypt = _encoder.GetBytes(data);
                var encryptedByteArray = rsa.Encrypt(dataToEncrypt, false).ToArray();
                var length = encryptedByteArray.Count();
                var item = 0;
                var sb = new StringBuilder();

                foreach (var x in encryptedByteArray)
                {
                    item++;
                    sb.Append(x);
                    if (item < length) sb.Append(",");
                }

                return sb.ToString();
            }
            catch (Exception)
            {
                throw new RSAException();
            }
        }

        public class RSAException : Exception
        {
            public RSAException() : base("RSA Encryption Error") { }
        }
    }
}