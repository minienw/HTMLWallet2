# HTMLwallet
HTML wallet option for connecting to DCC validation services and uploading proof

# Usage guide
# Introduction
The QR wallet web app is a .NET Blazor server application which will process any incoming QR validation requests using the services provided in the uploaded QR data and the QR code uploaded by the user.
# Requirements
<ul>
<li>
Source code (from GIT)
</li>
<li>
Microsoft Visual studio code / Microsoft Visual studio
</li>
</ul>

# Installation
<ol>
<li>
    Make the necessary customizations
</li>
<li>
    Compile the application and deploy to any hosting suitable for <a href="https://docs.microsoft.com/en-us/aspnet/core/blazor/host-and-deploy/server?view=aspnetcore-6.0">.NET 6 applications</a> 
</li>
</ol>

# Usage
<p>The main page for the application is the \process endpoint in the application. Calling the \process endpoint with a base64 encoded json data set will start the QR validation process. The json format that is expected looks like this:</p
<code> 
{
      "protocol":"DCCVALIDATION",
      "protocolVersion":"1.0.0",
      "serviceIdentity":"https://dgca-booking-demo-eu-test.cfapps.eu10.hana.ondemand.com/api/identity ",
      "privacyUrl":"https://validation-decorator.example ",
      "token":"tokendata,
      "consent":"Please confirm to start the DCC Exchange flow. If you not confirm, the flow is aborted.",
      "subject":"318ca61e-90a3-43eb-912e-e40f1154cbe2",
      "serviceProvider":"Booking Demo"
      }
</code>

The full URL for the process page can look like this:
https://checkinqr.example.org/process/ewogICAgInByb3RvY29sIjogIkRDQ1ZBTElEQVRJT04iLAogICAgInByb3RvY29sVmVyc2lvbiI6ICIxLjAuMCIsCiAgICAic2VydmljZUlkZW50aXR5IjogImh0dHBzOi8vZGdjYS1ib29raW5nLWRlbW8tZXUtdGVzdC5jZmFwcHMuZXUxMC5oYW5hLm9uZGVtYW5kLmNvbS9hcGkvaWRlbnRpdHkiLAogICAgInByaXZhY3lVcmwiOiAiaHR0cHM6Ly92YWxpZGF0aW9uLWRlY29yYXRvci5leGFtcGxlIiwKICAgICJ0b2tlbiI6ICJleUowZVhBaU9pSktWMVFpTENKcmFXUWlPaUppVXpoRU1pOVhlalYwV1QwaUxDSmhiR2NpT2lKRlV6STFOaUo5LmV5SnBjM01pT2lKb2RIUndjem92TDJSblkyRXRZbTl2YTJsdVp5MWtaVzF2TFdWMUxYUmxjM1F1WTJaaGNIQnpMbVYxTVRBdWFHRnVZUzV2Ym1SbGJXRnVaQzVqYjIwdllYQnBMMmxrWlc1MGFYUjVJaXdpWlhod0lqb3hOak00T0RnM01qQTBMQ0p6ZFdJaU9pSmhaalExTkRZd05TMDNZemxsTFRSaE56SXRZVGRpWlMxaE9USmlaR1E0TnpNek56QWlmUS44bHV0bDlEbE9FRGFIb0JZNEVtYmpZcW5qUzZtdUNUenN1a29VSDBhU3pnWjZfY0cyR3hpaW9lbTdxeVFkQXJITXB0bGoyRTFzenRXVEppQk9kQ3QzZyIsCiAgICAiY29uc2VudCI6ICJQbGVhc2UgY29uZmlybSB0byBzdGFydCB0aGUgRENDIEV4Y2hhbmdlIGZsb3cuIElmIHlvdSBub3QgY29uZmlybSwgdGhlIGZsb3cgaXMgYWJvcnRlZC4iLAogICAgInN1YmplY3QiOiAiYWY0NTQ2MDUtN2M5ZS00YTcyLWE3YmUtYTkyYmRkODczMzcwIiwKICAgICJzZXJ2aWNlUHJvdmlkZXIiOiAiQm9va2luZyBEZW1vIgp9

The application contains a test page in which the json data can be uploaded or a QR code containing the json data can be uploaded.


# Customization
 <p>By default the application will operate in demonstration and debug mode. This means the data retrieved from the validation services is outputted to the users screen. Below instructions provide some means to customize the application:</p>     

<ol>
<li>
    Layout: go to https://mudblazor.com/ for means to change the layout;
</li>
<li>
    Show/Hide debug panel: <br/>
      a. Open the process.razor file <br/>
      b. To remove: remove the line containing the SimpleAudit component<br/>
      c. To add: the SimpleAudit component just above the @code{ tag      
</li>
</ol>
