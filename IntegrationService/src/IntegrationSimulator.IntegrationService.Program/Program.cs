// See https://aka.ms/new-console-template for more information

using IntegrationSimulator.IntegrationService.Application.Services;
using IntegrationSimulator.IntegrationService.Domain.Interfaces;
using IntegrationSimulator.IntegrationService.Infrastructure.HttpClients;

Console.WriteLine("Hello, World!");

var httpClient1  = new HttpClient();
var httpClient2 = new HttpClient();

IJobAdClient jobClient = new PlatsbankenClient(httpClient1);
IFakeERPClient erpClient = new FakeERPClient(httpClient2);

IPollingCoordinator coordinator = new PollingCoordinator(erpClient, jobClient);

await coordinator.Execute();
