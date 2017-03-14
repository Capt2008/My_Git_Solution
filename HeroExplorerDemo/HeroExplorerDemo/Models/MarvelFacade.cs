using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Streams;

namespace HeroExplorerDemo.Models
{
    public class MarvelFacade
    {
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //      一个MarvelFacade类，它封装了HttpClient的功能，能够通过MarvelFacade.Interview() 方法来拿到MarvelApi的response 的string.它需具有如下Properties and Methods:
        //		c.一个能够生成MD5hash的方法，将timestamp+privatekey+publickey转成MD5string。
        //		d.一个request构造器，可以使用上述方法生成一个可供使用的httprequest。
        //      使用Httpclient发送request，并接收返回。
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private static string CreateRequest(int limit, int offset)
        {
            long ts = DateTime.UtcNow.Ticks;
            var reqStr = string.Format(
                "http://gateway.marvel.com:80/v1/public/characters?limit={0}&offset={1}&ts={2}&apikey={3}&hash={4}",
                limit,
                offset,
                ts.ToString(),
                MarvelKeys.PubliceKey,
                GetMD5Hash(ts)
                );
            return reqStr;
        }

        /// <summary>
        /// Create a MD5 Hashcode, orignal string is formated as timestamp+privateKey+publicKey
        /// ts - a timestamp (or other long string which can change on a request-by-request basis)
        /// hash - a md5 digest of the ts parameter, your private key and your public key(e.g.md5(ts+privateKey+publicKey)
        /// </summary>
        /// <param name="timeStamp">the same datetime.ticks as it in request ts value</param>
        /// <returns></returns>
        private static object GetMD5Hash(long timeStamp)
        {
            HashAlgorithmProvider encryptor = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            var md5Hash = encryptor.CreateHash();
            // MarvelKeys is my MarvelApi Developer Keys, not suiltable for upload
            string inputMsg = string.Format("{0}{1}{2}", timeStamp.ToString(), MarvelKeys.PrivateKey, MarvelKeys.PubliceKey);
            IBuffer inputBuffer = CryptographicBuffer.ConvertStringToBinary(inputMsg, BinaryStringEncoding.Utf8);
            md5Hash.Append(inputBuffer);
            IBuffer outputBuffer = md5Hash.GetValueAndReset();
            return CryptographicBuffer.EncodeToHexString(outputBuffer);
        }

        public static async Task<CharacterDataModel> GetCharacterDataModeAsync(int limit, int offset)
        {
            HttpClient client = new HttpClient();
            var responseStream = await client.GetStreamAsync(CreateRequest(limit, offset));
            var serializer = new DataContractJsonSerializer(typeof(CharacterDataModel));
            return serializer.ReadObject(responseStream) as CharacterDataModel;
        }

        public static async Task<List<Character>> GetCharactersAsync(int limit, int offset)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(CreateRequest(limit, offset));
            // var path=await SaveJsonFile(response);
            var ms = new MemoryStream();
            await response.Content.CopyToAsync(ms);            
            var serializer = new DataContractJsonSerializer(typeof(CharacterDataModel));
            ms.Position = 0;
            var characterData = serializer.ReadObject(ms);
            return ((CharacterDataModel)characterData).data.results;
        }

        private static async Task<string> SaveJsonFile(HttpResponseMessage response)
        {
            var datafile = await ApplicationData.Current.LocalFolder.CreateFileAsync("JsonData" + DateTime.Now.Minute);
            var stream = await datafile.OpenStreamForWriteAsync();
            await response.Content.CopyToAsync(stream);
            return datafile.Path;
        }
    }
}
