using IntegrationSimulator.IntegrationService.Domain.Interfaces;
using IntegrationSimulator.IntegrationService.Domain.Models;

namespace IntegrationSimulator.IntegrationService.Application.Services;

public class PollingCoordinator : IPollingCoordinator
{
    private readonly IFakeERPClient _erpClient;
    private readonly IJobAdClient _jobAdClient;
    //TODO: Add some kind of dataaccess to fetch meta data about past polling attempt to determine latest successfull poll date

    public PollingCoordinator(IFakeERPClient erpClient, IJobAdClient jobAdClient)
    {
        _erpClient = erpClient;
        _jobAdClient = jobAdClient;
    }

    public async Task Execute()
    {
        //TODO: replace this variable with the actual date from the meta data store
        DateTime latestSuccessfullFetch = DateTime.UtcNow.AddDays(-10);

        var ads = await _jobAdClient.GetNewAdListingsAsync(latestSuccessfullFetch);

        //TODO: Log how many found
        if (ads.NumberOfAds != 0)
        {
            var dto = ads.Ads
                .Select(x => new PostNewJobToFakeERP(x.Id, x.Title, x.WorkplaceName, x.PublishedDate))
                .ToList();
            await _erpClient.PostNewJobListingsAsync(dto);

            //TODO: Log how many sent
            return;
        }
        //TODO: Log zero sent

    }
}