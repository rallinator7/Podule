using System;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Util;
using Amazon.S3.Model;
using WebApi.Models;
using Amazon.S3.Transfer;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace WebApi.Services
{
    public interface IS3Service
    {
        Task<S3Response> CreateLockerAsync(string LockerName);
        Task AddPoduleAsync(string LockerName, string FileName, IFormFile File);
        Task<byte[]> GetPoduleAsync(string LockerName, string FileName);
    }

    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _S3Settings;

        public S3Service(IAmazonS3 S3Settings)
        {
            _S3Settings = S3Settings;
        }

        public async Task<S3Response> CreateLockerAsync(string LockerName)
        {
            try
            {
                if (await AmazonS3Util.DoesS3BucketExistV2Async(_S3Settings, LockerName) == false)
                {
                    var putLockerRequest = new PutBucketRequest
                    {
                        BucketName = LockerName,
                        UseClientRegion = true
                    };

                    var response = await _S3Settings.PutBucketAsync(putLockerRequest);

                    return new S3Response
                    {
                        Message = response.ResponseMetadata.RequestId,
                        Status = response.HttpStatusCode
                    };
                }
            }
            catch (AmazonS3Exception e)
            {
                return new S3Response
                {
                    Status = e.StatusCode,
                    Message = e.Message
                };
            }
            return new S3Response
            {
                Status = System.Net.HttpStatusCode.InternalServerError,
                Message = ":("
            };
        }

        public async Task AddPoduleAsync(string LockerName, string FileName, IFormFile File)
        {
            try
            {
                var fileTransferUtility = new TransferUtility(_S3Settings);

                var FileTransferUtilityRequest = new TransferUtilityUploadRequest
                {
                    BucketName = LockerName,
                    StorageClass = S3StorageClass.IntelligentTiering,
                    Key = FileName,
                    ContentType = File.ContentType,
                    InputStream = File.OpenReadStream(),
                    CannedACL = S3CannedACL.NoACL,
                    PartSize = File.Length
                };


                await fileTransferUtility.UploadAsync(FileTransferUtilityRequest);
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error Encountered on Server. Message:'{0}' wher writing an object.", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Encountered on Server. Message:'{0}' wher writing an object.", e.Message);
            }
        }

        public async Task<byte[]> GetPoduleAsync(string LockerName, string FileName)
        {
            byte[] Bytes;

            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = LockerName,
                    Key = FileName
                };

                using (var response = await _S3Settings.GetObjectAsync(request))
                using (var responseStream = response.ResponseStream)
                using (var reader = new StreamReader(responseStream))
                {
                    Bytes = Encoding.ASCII.GetBytes(reader.ReadToEnd());
                }



                return Bytes;

            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error Encountered on Server. Message:'{0}' wher writing an object.", e.Message);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Encountered on Server. Message:'{0}' wher writing an object.", e.Message);
                return null;
            }
        }

    }
}
