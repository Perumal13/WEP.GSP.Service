/*******************************************************************************
 * 			Miracle Accounting Software - RKIT Software Pvt. Ltd.
 *******************************************************************************
 * You may amend and distribute as you like, but don't remove this header!
 *
 * MiracleGST provides demo of GST API Call. 
 *
 * Copyright (C) 2017  RKIT Software Pvt. Ltd.
 *
 * This Program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.

 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  
 * See the GNU Lesser General Public License for more details.
 *
 * The GNU Lesser General Public License can be viewed at http://www.opensource.org/licenses/lgpl-license.php
 * If you unfamiliar with this license or have questions about it, here is an http://www.gnu.org/licenses/gpl-faq.html
 *
 * All code and executables are provided "as is" with no warranty either express or implied. 
 * The author accepts no liability for any damage or loss of business that this product may cause. 
 *******************************************************************************/
using System;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography;
//using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WEP.GSP.Document;
namespace WEP.GSP.Business
{
    public class EncryptionUtils
    {

        public  byte[] CreateKey()
        {
            AesCryptoServiceProvider crypto = new AesCryptoServiceProvider();
            crypto.KeySize = 256;
            crypto.GenerateKey();
            byte[] key = crypto.Key;
            return key;
        }

        public  string HMAC_Encrypt(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var HMACSHA256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = HMACSHA256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }

        public static string HMAC_Encrypt(byte[] messageBytes, byte[] keyByte)
        {
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }

        public static string GenerateHMAC(string message, byte[] EK)
        {
            using (var HMACSHA256 = new HMACSHA256(EK))
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                byte[] hashmessage = HMACSHA256.ComputeHash(data);
                return Convert.ToBase64String(hashmessage);
            }
        }

        public static string HMAC_Encrypt(byte[] EK)
        {
            using (var HMACSHA256 = new HMACSHA256())
            {
                byte[] hashmessage = HMACSHA256.ComputeHash(EK);
                return Convert.ToBase64String(hashmessage);
            }
        }        

        public static string SHA256Checksum(string plainText)
        {
            using (var sha1 = SHA256.Create())
            {
                byte[] outputBytes = sha1.ComputeHash(Encoding.ASCII.GetBytes(plainText));
                return BitConverter.ToString(outputBytes).Replace("-", "").ToLower().Trim();
            }
        }             

        public static string Encrypt(string plainText, byte[] keyBytes)
        {
            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(plainText);
            AesManaged tdes = new AesManaged();
            tdes.KeySize = 256;
            tdes.BlockSize = 128;
            tdes.Key = keyBytes;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform crypt = tdes.CreateEncryptor();
            byte[] cipher = crypt.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
            tdes.Clear();
            return Convert.ToBase64String(cipher, 0, cipher.Length);
        }

        public static string Encrypt(string plainText, string key)
        {
            byte[] data = UTF8Encoding.UTF8.GetBytes(plainText);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            return Encrypt(data, keyBytes);
        }

        public static string Encrypt(byte[] data, byte[] keys)
        {
            AesManaged tdes = new AesManaged();
            tdes.KeySize = 256;
            tdes.BlockSize = 128;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            tdes.Key = keys;
            ICryptoTransform crypt = tdes.CreateEncryptor();
            byte[] cipher = crypt.TransformFinalBlock(data, 0, data.Length);
            return Convert.ToBase64String(cipher, 0, cipher.Length);
        }

        public static byte[] Decrypt(string encryptedText, string key)
        {
            byte[] dataToDecrypt = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            return Decrypt(dataToDecrypt, keyBytes);
        }

        public static byte[] Decrypt(string encryptedText, byte[] keys)
        {
            byte[] dataToDecrypt = Convert.FromBase64String(encryptedText);
            return Decrypt(dataToDecrypt, keys);
        }

        public static byte[] Decrypt(byte[] dataToDecrypt, byte[] keys)
        {
            AesManaged tdes = new AesManaged();
            tdes.KeySize = 256;
            tdes.BlockSize = 128;
            tdes.Key = keys;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform decrypt__1 = tdes.CreateDecryptor();
            byte[] deCipher = decrypt__1.TransformFinalBlock(dataToDecrypt, 0, dataToDecrypt.Length);
            tdes.Clear();
            return deCipher;
        }

        public  string EncryptTextWithPublicKey(string input)
        {
            byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(input);
            return EncryptTextWithPublicKey(bytesToBeEncrypted);
        }

        private static readonly byte[] Salt = new byte[] { 10, 20, 30, 40, 50, 60, 70, 80 };

        private static string binPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

        
        public static X509Certificate2 getPublicKey()
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            string Certificate_Path = binPath + "\\" + Constants.GstCertificate;
            //X509Certificate2 cert2 = new X509Certificate2(Path.Combine(Application.StartupPath, "GSTN_PublicKey.cer"));
            X509Certificate2 cert2 = new X509Certificate2(Certificate_Path); ;
            return cert2;
        }

        public  string EncryptTextWithPublicKey(byte[] bytesToBeEncrypted)
        {
            X509Certificate2 certificate = getPublicKey();
            RSACryptoServiceProvider RSA = (RSACryptoServiceProvider)certificate.PublicKey.Key;
            byte[] bytesEncrypted = RSA.Encrypt(bytesToBeEncrypted, false);
            string result = Convert.ToBase64String(bytesEncrypted);
            return result;

            //string GetKey = File.ReadAllText(Path.Combine(Application.StartupPath, "GSTN_private.pem")).Replace("-----BEGIN RSA PRIVATE KEY-----", "").Replace("-----END RSA PRIVATE KEY-----", "");
            //RSACryptoServiceProvider rsa = DecodeX509PublicKey(Convert.FromBase64String(GetKey));
            //byte[] bytesEncrypted = rsa.Encrypt(bytesToBeEncrypted, false);
            //string result = Convert.ToBase64String(bytesEncrypted);
            //return result;
        }

        public static string EncryptTextWithPublicKey(string input, string certificateLocation)
        {
            byte[] bytesEncrypted;
            byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(input);
            string GetKey = File.ReadAllText(certificateLocation).Replace("-----BEGIN RSA PUBLIC KEY-----", "").Replace("-----END RSA PUBLIC KEY-----", "");
            RSACryptoServiceProvider rsa = DecodeX509PublicKey(Convert.FromBase64String(GetKey));
            bytesEncrypted = rsa.Encrypt(bytesToBeEncrypted, false);
            string result = Convert.ToBase64String(bytesEncrypted);
            return result;
        }

        private static RSACryptoServiceProvider DecodeX509PublicKey(byte[] x509key)
        {
            // encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
            byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] seq = new byte[15];
            // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
            MemoryStream mem = new MemoryStream(x509key);
            BinaryReader binr = new BinaryReader(mem);    //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;

            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();   //advance 2 bytes
                else
                    return null;

                seq = binr.ReadBytes(15);       //read the Sequence OID
                if (!CompareBytearrays(seq, SeqOID))    //make sure Sequence for OID is correct
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8103) //data read as little endian order (actual data order for Bit String is 03 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8203)
                    binr.ReadInt16();   //advance 2 bytes
                else
                    return null;

                bt = binr.ReadByte();
                if (bt != 0x00)     //expect null byte next
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();   //advance 2 bytes
                else
                    return null;

                twobytes = binr.ReadUInt16();
                byte lowbyte = 0x00;
                byte highbyte = 0x00;

                if (twobytes == 0x8102) //data read as little endian order (actual data order for Integer is 02 81)
                    lowbyte = binr.ReadByte();  // read next bytes which is bytes in modulus
                else if (twobytes == 0x8202)
                {
                    highbyte = binr.ReadByte(); //advance 2 bytes
                    lowbyte = binr.ReadByte();
                }
                else
                    return null;
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };   //reverse byte order since asn.1 key uses big endian order
                int modsize = BitConverter.ToInt32(modint, 0);

                byte firstbyte = binr.ReadByte();
                binr.BaseStream.Seek(-1, SeekOrigin.Current);

                if (firstbyte == 0x00)
                {   //if first byte (highest order) of modulus is zero, don't include it
                    binr.ReadByte();    //skip this null byte
                    modsize -= 1;   //reduce modulus buffer size by 1
                }

                byte[] modulus = binr.ReadBytes(modsize);   //read the modulus bytes

                if (binr.ReadByte() != 0x02)            //expect an Integer for the exponent data
                    return null;
                int expbytes = (int)binr.ReadByte();        // should only need one byte for actual exponent data (for all useful values)
                byte[] exponent = binr.ReadBytes(expbytes);

                // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSAParameters RSAKeyInfo = new RSAParameters();
                RSAKeyInfo.Modulus = modulus;
                RSAKeyInfo.Exponent = exponent;
                RSA.ImportParameters(RSAKeyInfo);
                return RSA;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                binr.Close();
            }
        }

        private static bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }

        public static string Sign(string text, X509Certificate2 xcert = null)
        {
            X509Certificate2 mycert = null;
            X509Store my = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            my.Open(OpenFlags.ReadOnly);
            foreach (X509Certificate2 cert in my.Certificates)
            {
                if (cert.Subject.Contains("CN=Digant Rasiklal Vora"))
                {
                    mycert = cert;
                    break;
                }
            }
            RSACryptoServiceProvider csp = (RSACryptoServiceProvider)mycert.PrivateKey;
            if (csp == null)
            {
                throw new Exception("No valid cert was found");
            }
            // This one can:
            //RSACryptoServiceProvider csp2 = new RSACryptoServiceProvider();
            //csp2.ImportParameters(csp.ExportParameters(true));
            byte[] data = Encoding.UTF8.GetBytes(text);
            byte[] signature = csp.SignData(data, CryptoConfig.CreateFromName("SHA256"));
            bool isValid = csp.VerifyData(data, CryptoConfig.CreateFromName("SHA256"), signature);
            return Convert.ToBase64String(signature);
        }

        //public static string Sign(string signData)
        //{
        //    X509Certificate2 mycert = null;
        //    X509Store my = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        //    my.Open(OpenFlags.ReadOnly);
        //    foreach (X509Certificate2 cert in my.Certificates)
        //    {
        //        if (cert.Subject.Contains("CN=Digant Rasiklal Vora"))
        //        {
        //            mycert = cert;
        //            break;
        //        }
        //    }
        //    string Base64Payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(signData));
        //    string sha256 = SHA256Checksum(signData);
        //    byte[] data = Encoding.UTF8.GetBytes(sha256);
        //    // setup the data to sign
        //    ContentInfo content = new ContentInfo(data);
        //    SignedCms signedCms = new SignedCms(content);
        //    CmsSigner signer = new CmsSigner(mycert);
        //    signer.DigestAlgorithm = new Oid("SHA256");
        //    signer.IncludeOption = X509IncludeOption.WholeChain;
        //    // create the signature
        //    signedCms.ComputeSignature(signer, false);
        //    bool isValid = Verify(signedCms.Encode(), mycert);
        //    return Convert.ToBase64String(signedCms.Encode());
        //}

        //public static bool Verify(byte[] signature, X509Certificate2 certificate)
        //{
        //    if (signature == null)
        //        throw new ArgumentNullException("signature");
        //    if (certificate == null)
        //        throw new ArgumentNullException("certificate");
        //    // decode the signature
        //    SignedCms verifyCms = new SignedCms();
        //    verifyCms.Decode(signature);
        //    // verify it
        //    try
        //    {
        //        verifyCms.CheckSignature(new X509Certificate2Collection(certificate), false);
        //        return true;
        //    }
        //    catch (CryptographicException)
        //    {
        //        return false;
        //    }
        //}

        public static byte[] Compress(byte[] data)
        {
            using (var compressedStream = new MemoryStream())
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                zipStream.Write(data, 0, data.Length);
                zipStream.Close();
                return compressedStream.ToArray();
            }
        }

        public static byte[] Decompress(byte[] data)
        {
            using (var compressedStream = new MemoryStream(data))
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }
    }
}
