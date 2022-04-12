using System;
using System.Diagnostics;
using System.Net.Http;
using CheckInQrWeb.Core;
using CheckInQrWeb.Core.Models;
using Moq;
using Org.BouncyCastle.Crypto;
using Xunit;

namespace CheckIn.Shared.Tests
{
    //NB these tests are INTEGRATION tests and do not succeed unless both the airline stub and validation service (and redis server) are running.
    public class ThirdPartyServicesTests
    {
        //Remember to get a new token from whichever env u r testing
        //private const string QrCodeContentsBase64 = "eyJwcm90b2NvbCI6IkRDQ1ZBTElEQVRJT04iLCJwcm90b2NvbFZlcnNpb24iOiIyLjAwIiwic2VydmljZUlkZW50aXR5IjoiaHR0cDovL2xvY2FsaG9zdDo4MDgxL2lkZW50aXR5IiwicHJpdmFjeVVybCI6InByaXZhY3kgcG9saWN5IHVybC4uLiIsInRva2VuIjoiZXlKcmFXUWlPaUpNTmxSamFtNWlXbWRsTkQwaUxDSmhiR2NpT2lKU1V6STFOaUo5LmV5SnBjM04xWlhJaU9pSm9kSFJ3T2k4dmEyVnNiR0ZwY2k1amIyMGlMQ0pwWVhRaU9qRXdNREF3TURBd0xDSnpkV0lpT2lJd01USXpORFUyTnpnNVFVSkRSRVZHTURFeU16UTFOamM0T1VGQ1EwUkZSaUlzSW1WNGNDSTZNakF3TURBd01EQjkuSXcwMzNwT0RoMkpLckRaVzEwVHEyX3dBemxUaG5UbW5ZQmxQdHlhNXdyVjQ0LTA4ZHBSN2hIbnNzRDBSekVFZ3dPck5DeEt0NWkyY2p3SFZpVDhWbXpWelFVTy1ZOXR6aERTaFZ6aWZWZDhTRVU2VzZ0OWc4Z21SZE1KWFhPd2tGc0Y1Y0dCMUdDc0JfcjFoRGVLcF81Q3hEbXc5RDRKX2NQNDF1MVNqc3UtZUZLN2lBSm5BaUpsbUZpOHNneE9JQ0hGelFHWmswanBwRmlhQXhCSm1qdVUwR0hkUWJLNk9RT09LR29MUmVNYXNneS1uYk5yOUFNcHFoRnRlRFVheWlQODFVelBFdFctcGdLaE5IaHp6RHU4NW5DNkMzZG1DRUd2aDkxNUJ4WWZBNElhZ1JXd21YeGtEX3Y2dlVFWnVXM0Ftb3l0SHJfcURRSnNYbnRLNVB5cFRnMmNRbVhBZFBZWUhyNjBESWtHTFJ3eWYxa3hYam9TTjNBeVYyUDFIOEotZmRmanRCZnFuelp0VnFVTk1tajV0b24yOEtvSDlsVFdYeDBoT01iTEZaNDlMU3NMTG9UNzlNYTROeklWTG5ZQkc4aVdqem1ZTG1aYlJpeGk5ZU9VUmI3ZlNRYi12ZXhHTTF0LS1EajM1WWVNUXp6Nkx1UEVjXzViYl8xYVotZEFCSHI2VExKTTBnSTJhNURFVW5ObW11RFNlLWZCd2pERXBrTDh2M1BzcGF0OVhGekhqQWNYT1Y3OGJvam93UzdLY2V6cFJRTU1ZdnR4NTFFVWsyNnl5ckdHU3JjMEdDNFJTaTUyUUFuYkc5QllHMWh0NEluVnhPOVY3ZWRHSmdMWXV5VnZRdGJ4R1U2cEJTN05uMUtLbzE4VmJ5aWNhQkNmR0h0VHVycmMiLCJjb25zZW50IjoiaW5mb3JtZWQgY29uc2VudCB0ZXh0Li4uIiwic3ViamVjdCI6IjAxMjM0NTY3ODlBQkNERUYwMTIzNDU2Nzg5QUJDREVGIiwic2VydmljZVByb3ZpZGVyIjoiS2VsbGFpciJ9";
        //e.g. localhost:8081/starthere2
        private const string QrCodeContentsBase64 = "eyJwcm90b2NvbCI6IkRDQ1ZBTElEQVRJT04iLCJwcm90b2NvbFZlcnNpb24iOiIyLjAwIiwic2VydmljZUlkZW50aXR5IjoiaHR0cDovL2xvY2FsaG9zdDo4MDgxL2lkZW50aXR5IiwicHJpdmFjeVVybCI6InByaXZhY3kgcG9saWN5IHVybC4uLiIsInRva2VuIjoiZXlKcmFXUWlPaUpNTmxSamFtNWlXbWRsTkQwaUxDSmhiR2NpT2lKU1V6STFOaUo5LmV5SnBjM04xWlhJaU9pSm9kSFJ3T2k4dmEyVnNiR0ZwY2k1amIyMGlMQ0pwWVhRaU9qRXdNREF3TURBd0xDSnpkV0lpT2lJd05rUkROVUpGUTBNeVJFWTBPVGMzT1RKQk9EUkJSRVZHTTBFNE56azROQ0lzSW1WNGNDSTZNakF3TURBd01EQjkuVngzc3BjakxuQVZlNUpEN0o4ZG9FVWtZMU1feDNqMVZoX1UteldpOFBnMmF3Uml3bG52YVJHdHgwNVB0NmNhLVoyd200V3l3aEh6allBVFBZeXNmSGx1NW5TcXBiNlpGS2JYQ1VqRXhZczB1T0liQzd0SThWYlBhbHZmeDBqNFZ0V051OVh1bVB5QjdaSmlLWkp6cXdTdTFBWkNqNHNQbEZxVURyQk1qcDMteEtaeWVSUW9iNzlldXlRQlI5dG5CaTY3UWZjWnlCbU54d20wMWFQSC15c05NRG5hME5sbG40RlBiaWRHRUtiRXNnNDVOWkthckFmNk40REhHR1htMGtWRzQ2V2VpRG81c3VQUUNUcl9nNWtBWWZnakxyR0c0NlZFUHFmNnFuN0sxb0RZWGE3Z25RYWhvMFRJWWQyYTlxLVFaUGxib2VhQ0M5SkdUWnFrWGVOVWdLbmlWRk04LXVGanltdEZBT21WNVFJaUtGbUpnZ1Z4Ym44QzFvZU5tMklfWUVjS05ZcVlBU0FiekFZVmFUMHJkM1VUWHFXdm1yZHBiQllXVHFMSXpSczFUTi14cDM3ZWhGWUxwa1Zud3VqMndxMVltYkNBUlJRZ08zcHgyc0lBNE9xdjEzS0JQSF92QkJ3RWROaHRncHpGNXdLWFVRU1dnZFJ6aXNsS0laRkJRb3hQb3FQeG1rRHB5YUM0bXhIaHV4Rk1heTFDYWxyQTVnV295YjZORnpoenpGVUNjRXAtdWViUGdKX3RNaDJwSlQwYXRTOWV5TGNHVUt2ZzFZX1BxLTI0NVNTczAyVXJ0cUhEMGdIWDhibE9Ib1JjbVNfMkhPbkdkNUxfZUFFYm9ibGs2dzJoanhrZkFtZnlfRFJOMDZnUmMxa1d2OXVrUUQ5M3MxQlEiLCJjb25zZW50IjoiQnkgY2xpY2tpbmcg4oCcVXBsb2Fk4oCdIGFuZCBzZWxlY3RpbmcgYSBRUiBjb2RlIHlvdSB3aWxsIGJlIHNlbmRpbmcgeW91IERDQyBjb250YWluaW5nIHBlcnNvbmFsIGRhdGEgdG8gdGhlIHNlcnZlciB0aGF0IHdpbGwgdmFsaWRhdGUgaXQgZm9yIHlvdXIgdHJhdmVsLiBNYWtlIHN1cmUgeW91IGV4cGVjdCB0byBkbyBzby4gSWYgeW91IGFyZSBub3QgY2hlY2tpbmcgaW4gZm9yIGEgdHJpcCBhYnJvYWQsIGNsb3NlIHlvdXIgYnJvd3NlciBzY3JlZW4uO0J5IHNlbGVjdGluZyBPSyB5b3Ugd2lsbCBiZSBzZW5kaW5nIHRoZSB2YWxpZGF0aW9uIHJlc3VsdCBjb250YWluZyBwZXJzb25hbCBkYXRhIHRvIHRoZSB0cmFuc3BvcnQgY29tcGFueS4gT25seSBkbyBzbyBpZiB5b3UgYXJlIGFjdHVhbGx5IGNoZWNraW5nIGluLiIsInN1YmplY3QiOiIwNkRDNUJFQ0MyREY0OTc3OTJBODRBREVGM0E4Nzk4NCIsInNlcnZpY2VQcm92aWRlciI6IktlbGxhaXIifQ==";
       
        private const string Dcc = "HC1:NCF7X3W08$$Q030:+HGHBFO026M6M4NHSU5S8CUHC9:R7YL8YW7.6JUVM0$U7UQ%-B/$5X$59498$BLQH*CT4TJ603TRTIHKK5H6%EJTEPTSIWEJFAYMPRMQ%BOPUGIL3P2CHZMZ35D:2G7JU5E0B180OZ0W7:O/C5TAVVO5X$D+BTE$C:EWP 1Z$29%IDM5-6Q+F56U0ZBI0*8UEADP2LBLM9QH7HWPIYMGCXA5773R7 HFOV0-VG::AP9POXD6-J/WM4*R*O7HZSJG3NYTLQFT5D51V 4C5D8M-JOYLNTC*%MG5LWI1 H7%DNGUQ%3RQ$H4DQULL905%*1ZV6S9GIUHL103BHVUVZU9 0I%:DVF1H21LCCLCB$W4HVN%2BN7CLK07BG/PS:W3$M91WI5N02 QASQNY81+6BJ9OFERUK-AHWU5I7L-%2ZY9J/G:-A/UFGIIETL6 G41W00JIRP-NMJLP.W7 7BHJ0J V%%HR/HMWQG+CVX9NQABH8129 G6KD38L4I6G$6WT.73ZR80WH+582DUZ1O3G.9NGWNTZJ*.6NCVLUR0IH2SLH.SMPU 0PL:MDAOENSYUIJ9NH9OK8BZ6S:38IME6OE*QVUSR1FW+:QU0";

        [Fact]
        public void AirlineIdentity()
        {
            var fac = new Moq.Mock<IHttpClientFactory>();
            fac.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(() => new HttpClient());

            var subject = new HttpGetIdentityCommand(fac.Object);
            var response = subject.ExecuteAsync("http://localhost:8081/identity").GetAwaiter().GetResult();
            Assert.True(response.id.StartsWith("http") && response.id.EndsWith("/identity"));
        }

        [Fact]
        public (AsymmetricCipherKeyPair keyPair, HttpPostTokenCommandResult response) AirlineToken()
        {
            var fac = new Moq.Mock<IHttpClientFactory>();
            fac.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new HttpClient());

            var keyPair = Crypto.GenerateEcKeyPair();

            var initiatingToken = QrCodeContentsBase64;
            var args = new HttpPostTokenCommandArgs
            {
                AccessTokenServiceUrl = "http://localhost:8081/token",
                InitiatingToken = initiatingToken,
                WalletPublicKey = keyPair.Public
            };

            var subject = new HttpPostTokenCommand(fac.Object);
            var response = subject.ExecuteAsync(args).GetAwaiter().GetResult();
            //Assert.Equal("http://localhost:8081/identity", id.id);

            return (keyPair, response);
        }

        [Fact]
        public void Validate()
        {
            var fac = new Moq.Mock<IHttpClientFactory>();
            fac.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(() => new HttpClient());

            var workflow = new VerificationWorkflow(
                new HttpPostTokenCommand(fac.Object),
                new HttpPostValidateCommand(fac.Object),
                new HttpGetIdentityCommand(fac.Object),
                new HttpPostCallbackCommand(fac.Object)
                );

            var r0 = workflow.OnInitialized(QrCodeContentsBase64);
            Assert.True(r0.Success);

            var r1 = workflow.ValidateDccAsync(Dcc).GetAwaiter().GetResult();

            //TODO need better test data BUT the services are producing the appropriate results
            Assert.False(r1.Success);
            Trace.WriteLine(r1.Message);
            Assert.True(r1.Message.StartsWith("Security token received from DCC va"));

            var r2 = workflow.NotifyServiceProvider().GetAwaiter().GetResult();
            Assert.True(r2.Success);
        }
    }
}