﻿using System.Threading.Tasks;
using RX.Nyss.Common.Configuration;
using RX.Nyss.Common.Utils;

namespace RX.Nyss.Common.Services
{
    public interface IDataBlobService
    {
        Task StorePlatformAgreement(string sourceAgreement, string blobName);
    }

    public class DataBlobService : IDataBlobService
    {
        private readonly IConfig _config;
        private readonly BlobProvider _dataBlobProvider;

        public DataBlobService(IConfig config)
        {
            _config = config;
            _dataBlobProvider = new BlobProvider(config.PlatformAgreementsContainerName, config.ConnectionStrings.DataBlobContainer);
        }

        public async Task StorePlatformAgreement(string sourceAgreement, string blobName) => await _dataBlobProvider.CopyBlob(sourceAgreement, blobName);
    }
}
