using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EppLib.Extensions.Nominet.DomainCheck;
using EppLib.Extensions.Nominet.Notifications;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using EppLib.Entities;

namespace EppLib.Tests
{
    [TestClass]
    public class LocalTest
    {
        private TestContext _context;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get { return _context; }
            set { _context = value; }
        }

        public LocalTest()
        {
        }

        // Misc EPP Commands

        #region Login

        /// <summary>
        /// Login Command, example RFC5730
        /// </summary>
        [TestMethod]
        [TestCategory("LocalCommand")]
        [DeploymentItem("TestData/LoginCommand1.xml")]
        public void TestLoginCommand1()
        {
            string expected = File.ReadAllText("LoginCommand1.xml");

            var command = new Login("ClientX", "foo-BAR2", "bar-FOO2");
            command.TransactionId = "ABC-12345";
            command.Options = new Options() { MVersion = "1.0", MLang = "en" };

            var services = new Services();
            command.Services = services;

            services.ObjURIs.Add("urn:ietf:params:xml:ns:obj1");
            services.ObjURIs.Add("urn:ietf:params:xml:ns:obj2");
            services.ObjURIs.Add("urn:ietf:params:xml:ns:obj3");
            services.Extensions.Add("http://custom/obj1ext-1.0");

            Assert.AreEqual(expected, command.ToXml().InnerXml);
        }

        /// <summary>
        /// Login response, example RFC5730
        /// </summary>
        [TestMethod]
        [TestCategory("LocalResponse")]
        [DeploymentItem("TestData/LoginResponse1.xml")]
        public void TestLoginResponse1()
        {
            byte[] input = File.ReadAllBytes("LoginResponse1.xml");
            var response = new LoginResponse(input);

            Assert.AreEqual("1000", response.Code);
            Assert.AreEqual("Command completed successfully", response.Message);
            Assert.AreEqual("ABC-12345", response.ClientTransactionId);
            Assert.AreEqual("54321-XYZ", response.ServerTransactionId);
        }
        #endregion

        #region Logout

        /// <summary>
        /// Logout Command, example RFC5730
        /// </summary>
        [TestMethod]
        [TestCategory("LocalCommand")]
        [DeploymentItem("TestData/LogoutCommand1.xml")]
        public void TestLogoutCommand1()
        {
            string expected = File.ReadAllText("LogoutCommand1.xml");

            var command = new Logout();
            command.TransactionId = "ABC-12345";

            Assert.AreEqual(expected, command.ToXml().InnerXml);
        }

        /// <summary>
        /// Logout response, example RFC5730
        /// </summary>
        [TestMethod]
        [TestCategory("LocalResponse")]
        [DeploymentItem("TestData/LogoutResponse1.xml")]
        public void TestLogoutResponse1()
        {
            byte[] input = File.ReadAllBytes("LogoutResponse1.xml");
            var response = new LoginResponse(input);

            Assert.AreEqual("1500", response.Code);
            Assert.AreEqual("Command completed successfully; ending session", response.Message);
            Assert.AreEqual("ABC-12345", response.ClientTransactionId);
            Assert.AreEqual("54321-XYZ", response.ServerTransactionId);
        }
        #endregion

        #region Hello

        /// <summary>
        /// Hello Command, example RFC5730
        /// </summary>
        [TestMethod]
        [TestCategory("LocalCommand")]
        [DeploymentItem("TestData/Hello1.xml")]
        public void TestHelloCommand1()
        {
            string expected = File.ReadAllText("Hello1.xml");

            var command = new Hello();

            Assert.AreEqual(expected, command.ToXml().InnerXml);
        }

        /// <summary>
        /// Hello response (greeting), example RFC5730
        /// </summary>
        [TestMethod]
        [TestCategory("LocalResponse")]
        [DeploymentItem("TestData/Greeting1.xml")]
        public void TestHelloResponse1()
        {
            byte[] input = File.ReadAllBytes("Greeting1.xml");
            var response = new HelloResponse(input);

            Assert.IsNull(response.Code);
            Assert.IsNull(response.Message);

            /* TODO: Implement in HelloResponse
            Assert.AreEqual("Example EPP server epp.example.com", response.ServerId);
            Assert.AreEqual("2000-06-08T22:00:00.0Z", response.ServerDate);
            Assert.AreEqual("1.0", response.Version);
            List<string> expectedLanguages = new List<string>() { "en", "fr" };
            CollectionAssert.AreEqual(expectedLanguages.ToArray(), response.Languages.ToArray());
            Assert.IsTrue(response.Languages.Contains("en"));
            Assert.IsTrue(response.Languages.Contains("fr"));
            List<string> expectedObjURI = new List<string>()
                    {
                        "urn:ietf:params:xml:ns:obj1",
                        "urn:ietf:params:xml:ns:obj2",
                        "urn:ietf:params:xml:ns:obj3",
                    };
            CollectionAssert.AreEqual(expectedObjURI.ToArray(), response.ObjectNamespaces.ToArray());
            // Low priority: DCP, Data Collection Policy
            */

            Assert.IsNull(response.ClientTransactionId);
            Assert.IsNull(response.ServerTransactionId);
        }
        #endregion

        // Host

        #region Host Check
        /// <summary>
        /// Host check command, example RFC5732
        /// </summary>
        [TestMethod]
        [TestCategory("LocalCommand")]
        [DeploymentItem("TestData/HostCheckCommand1.xml")]
        public void TestHostCheckCommand1()
        {
            string expected = File.ReadAllText("HostCheckCommand1.xml");

            var command = new HostCheck(
                    new List<string>()
                    {
                        "ns1.example.com",
                        "ns2.example.com",
                        "ns3.example.com"
                    }
            );
            command.TransactionId = "ABC-12345";

            Assert.AreEqual(expected, command.ToXml().InnerXml);
        }

        /// <summary>
        /// Host check response, example RFC5732
        /// </summary>
        [TestMethod]
        [TestCategory("LocalResponse")]
        [DeploymentItem("TestData/HostCheckResponse1.xml")]
        public void TestHostCheckResponse1()
        {
            byte[] input = File.ReadAllBytes("HostCheckResponse1.xml");
            var response = new HostCheckResponse(input);

            Assert.AreEqual("1000", response.Code);
            Assert.AreEqual("Command completed successfully", response.Message);

            List<HostCheckResult> expectedResults = new List<HostCheckResult>()
                    {
                        new HostCheckResult()
                        {
                            Name = "ns1.example.com",
                            Available = true,
                            Reason = null
                        },
                        new HostCheckResult()
                        {
                            Name = "ns2.example2.com",
                            Available = false,
                            Reason = "In use"
                        },
                        new HostCheckResult()
                        {
                            Name = "ns3.example3.com",
                            Available = true,
                            Reason = null
                        }
                    };

            CollectionAssert.AreEqual(expectedResults.ToArray(), response.Results.ToArray());

            Assert.AreEqual("ABC-12345", response.ClientTransactionId);
            Assert.AreEqual("54322-XYZ", response.ServerTransactionId);
        }
        #endregion

        #region TODO Host Info

        /// <summary>
        /// Host info command, example RFC5732
        /// </summary>
        [TestMethod]
        [TestCategory("LocalCommand")]
        [DeploymentItem("TestData/HostInfoCommand1.xml")]
        public void TestHostInfoCommand1()
        {
            string expected = File.ReadAllText("HostInfoCommand1.xml");

            var command = new HostInfo("ns1.example.com");
            command.TransactionId = "ABC-12345";

            Assert.AreEqual(expected, command.ToXml().InnerXml);
        }

        /// <summary>
        /// Host info response, example RFC5732
        /// </summary>
        [TestMethod]
        [TestCategory("LocalResponse")]
        [DeploymentItem("TestData/HostInfoResponse1.xml")]
        public void TestHostInfoResponse1()
        {
            Assert.Inconclusive("Not implemented");

            /*
            byte[] input = File.ReadAllBytes("HostInfoResponse1.xml");
            var response = new HostInfoResponse(input);
            //var host = response.Host;
            var host = new Host();

            Assert.AreEqual("1000", response.Code);
            Assert.AreEqual("Command completed successfully", response.Message);

            Assert.AreEqual("ns1.example.com", host.HostName);

            Assert.AreEqual("NS1_EXAMPLE1-REP", host.Roid);
            Assert.AreEqual("ClientY", host.ClId);
            Assert.AreEqual("ClientX", host.CrId);
            Assert.AreEqual("ClientX", host.UpId);
            Assert.AreEqual("1999-04-03T22:00:00.0Z", host.CrDate);
            Assert.AreEqual("1999-12-03T09:00:00.0Z", host.UpDate);
            Assert.AreEqual("2000-04-08T09:00:00.0Z", host.TrDate);

            var status = new List<Status>()
                    {
                        new Status("linked", null),
                        new Status("clientUpdateProhibited", null)
                    };
            CollectionAssert.AreEqual(status.ToArray(), host.Status.ToArray());

            var hosts = new List<HostAddress>()
                    {
                        new HostAddress("192.0.2.2", "v4"),
                        new HostAddress("192.0.2.29", "v4"),
                        new HostAddress("1080:0:0:0:8:800:200C:417A", "v6")
                    };
            CollectionAssert.AreEqual(hosts.ToArray(), host.Addresses.ToArray());

            Assert.AreEqual("ABC-12345", response.ClientTransactionId);
            Assert.AreEqual("54322-XYZ", response.ServerTransactionId);
            */
        }

        #endregion

        #region TODO Host Create

        /// <summary>
        /// Host create command, example RFC5732
        /// </summary>
        [TestMethod]
        [TestCategory("LocalCommand")]
        [DeploymentItem("TestData/HostCreateCommand1.xml")]
        public void TestHostCreateCommand1()
        {
            string expected = File.ReadAllText("HostCreateCommand1.xml");

            Host host = new Host("ns1.example.com");
            host.Addresses.Add(new HostAddress("192.0.2.2", "v4"));
            host.Addresses.Add(new HostAddress("192.0.2.29", "v4"));
            host.Addresses.Add(new HostAddress("1080:0:0:0:8:800:200C:417A", "v6"));

            var command = new HostCreate(host);
            command.TransactionId = "ABC-12345";

            Assert.AreEqual(expected, command.ToXml().InnerXml);
        }

        /// <summary>
        /// Host create response, example RFC5732
        /// </summary>
        [TestMethod]
        [TestCategory("LocalResponse")]
        [DeploymentItem("TestData/HostCreateResponse1.xml")]
        public void TestHostCreateResponse1()
        {
            Assert.Inconclusive("Not implemented");
        }

        #endregion

        #region Host Update

        /// <summary>
        /// Host update command, example RFC5732
        /// </summary>
        [TestMethod]
        [TestCategory("LocalCommand")]
        [DeploymentItem("TestData/HostUpdateCommand1.xml")]
        public void TestHostUpdateCommand1()
        {
            string expected = File.ReadAllText("HostUpdateCommand1.xml");

            var command = new HostUpdate("ns1.example.com")
            {
                ToAdd = new EppHostUpdateAddRemove()
                {
                    Adresses = new List<HostAddress>()
                    {
                        new HostAddress("192.0.2.22", "v4")
                    },
                    Status = new List<Status>()
                    {
                        new Status(null, "clientUpdateProhibited")
                    }
                },
                ToRemove = new EppHostUpdateAddRemove()
                {
                    Adresses = new List<HostAddress>()
                    {
                        new HostAddress("1080:0:0:0:8:800:200C:417A", "v6")
                    },
                },
                HostChange = new HostChange()
                {
                    HostName = "ns2.example.com"
                }
            };

            command.TransactionId = "ABC-12345";

            Assert.AreEqual(expected, command.ToXml().InnerXml);
        }

        /// <summary>
        /// Host update response, example RFC5732
        /// </summary>
        [TestMethod]
        [TestCategory("LocalResponse")]
        [DeploymentItem("TestData/HostUpdateResponse1.xml")]
        public void TestHostUpdateResponse1()
        {
            byte[] input = File.ReadAllBytes("HostUpdateResponse1.xml");
            var response = new HostUpdateResponse(input);

            Assert.AreEqual("1000", response.Code);
            Assert.AreEqual("Command completed successfully", response.Message);

            Assert.AreEqual("ABC-12345", response.ClientTransactionId);
            Assert.AreEqual("54321-XYZ", response.ServerTransactionId);
        }

        #endregion

        #region TODO Host Delete

        /// <summary>
        /// Host delete command, example RFC5732
        /// </summary>
        [TestMethod]
        [TestCategory("LocalCommand")]
        [DeploymentItem("TestData/HostDeleteCommand1.xml")]
        public void TestHostDeleteCommand1()
        {
            Assert.Inconclusive("Not implemented");
        }

        /// <summary>
        /// Host delete response, example RFC5732
        /// </summary>
        [TestMethod]
        [TestCategory("LocalResponse")]
        [DeploymentItem("TestData/HostDeleteResponse1.xml")]
        public void TestHostDeleteResponse1()
        {
            Assert.Inconclusive("Not implemented");
        }

        #endregion

        // Contact

        #region Contact Check

        /// <summary>
        /// Contact check command, example RFC5733
        /// </summary>
        [TestMethod]
        [TestCategory("LocalCommand")]
        [DeploymentItem("TestData/ContactCheckCommand1.xml")]
        public void TestContactCheckCommand1()
        {
            string expected = File.ReadAllText("ContactCheckCommand1.xml");

            var command = new ContactCheck(
                    new List<string>()
                    {
                        "sh8013",
                        "sah8013",
                        "8013sah"
                    }
            );
            command.TransactionId = "ABC-12345";

            Assert.AreEqual(expected, command.ToXml().InnerXml);
        }

        /// <summary>
        /// Contact check response, example RFC5733
        /// </summary>
        [TestMethod]
        [TestCategory("LocalResponse")]
        [DeploymentItem("TestData/ContactCheckResponse1.xml")]
        public void TestContactCheckResponse1()
        {
            byte[] input = File.ReadAllBytes("ContactCheckResponse1.xml");
            var response = new ContactCheckResponse(input);

            Assert.AreEqual("1000", response.Code);
            Assert.AreEqual("Command completed successfully", response.Message);

            List<ContactCheckResult> expectedResults = new List<ContactCheckResult>()
                    {
                        new ContactCheckResult()
                        {
                            Id = "sh8013",
                            Available = true,
                            Reason = null
                        },
                        new ContactCheckResult()
                        {
                            Id = "sah8013",
                            Available = false,
                            Reason = "In use"
                        },
                        new ContactCheckResult()
                        {
                            Id = "8013sah",
                            Available = true,
                            Reason = null
                        }
                    };

            CollectionAssert.AreEqual(expectedResults.ToArray(), response.Results.ToArray());

            Assert.AreEqual("ABC-12345", response.ClientTransactionId);
            Assert.AreEqual("54322-XYZ", response.ServerTransactionId);
        }
        #endregion

        #region Contact Info

        /// <summary>
        /// Contact info command, example RFC5733
        /// </summary>
        [TestMethod]
        [TestCategory("LocalCommand")]
        [DeploymentItem("TestData/ContactInfoCommand1.xml")]
        public void TestContactInfoCommand1()
        {
            string expected = File.ReadAllText("ContactInfoCommand1.xml");

            var command = new ContactInfo("sh8013");
            command.TransactionId = "ABC-12345";
            command.Password = "2fooBAR";

            Assert.AreEqual(expected, command.ToXml().InnerXml);
        }

        /// <summary>
        /// Contact info response, example RFC5733
        /// </summary>
        [TestMethod]
        [TestCategory("LocalResponse")]
        [DeploymentItem("TestData/ContactInfoResponse1.xml")]
        public void TestContactInfoResponse1()
        {
            byte[] input = File.ReadAllBytes("ContactInfoResponse1.xml");
            var response = new ContactInfoResponse(input);
            var contact = response.Contact;

            Assert.AreEqual("1000", response.Code);
            Assert.AreEqual("Command completed successfully", response.Message);

            Assert.AreEqual("sh8013", contact.Id);

            Assert.AreEqual("+1.7035555555", contact.Voice.Value);
            Assert.AreEqual("1234", contact.Voice.Extension);
            Assert.AreEqual("+1.7035555556", contact.Fax.Value);
            Assert.AreEqual("jdoe@example.com", contact.Email);

            Assert.AreEqual("int", contact.PostalInfo.m_type);
            Assert.AreEqual("John Doe", contact.PostalInfo.m_name);
            Assert.AreEqual("Example Inc.", contact.PostalInfo.m_org);

            Assert.AreEqual("123 Example Dr.", contact.PostalInfo.m_address.Street1);
            Assert.AreEqual("Suite 100", contact.PostalInfo.m_address.Street2);
            Assert.IsNull(contact.PostalInfo.m_address.Street3);
            Assert.AreEqual("Dulles", contact.PostalInfo.m_address.City);
            Assert.AreEqual("VA", contact.PostalInfo.m_address.StateProvince);
            Assert.AreEqual("20166-6503", contact.PostalInfo.m_address.PostalCode);
            Assert.AreEqual("US", contact.PostalInfo.m_address.CountryCode);

            Assert.AreEqual("SH8013-REP", contact.Roid);
            Assert.IsTrue(contact.StatusList.Contains("linked"));
            Assert.IsTrue(contact.StatusList.Contains("clientDeleteProhibited"));
            Assert.AreEqual("ClientY", contact.ClId);
            Assert.AreEqual("ClientX", contact.CrId);
            Assert.AreEqual("ClientX", contact.UpId);
            Assert.AreEqual("1999-04-03T22:00:00.0Z", contact.CrDate);
            Assert.AreEqual("1999-12-03T09:00:00.0Z", contact.UpDate);
            Assert.AreEqual("2000-04-08T09:00:00.0Z", contact.TrDate);

            Assert.AreEqual("2fooBAR", contact.Password);
            Assert.AreEqual(Contact.DiscloseFlags.All & ~Contact.DiscloseFlags.Voice & ~Contact.DiscloseFlags.Email, contact.DiscloseMask);

            Assert.AreEqual("ABC-12345", response.ClientTransactionId);
            Assert.AreEqual("54322-XYZ", response.ServerTransactionId);
        }

        #endregion

        #region TODO Contact Update

        /// <summary>
        /// Contact update command, example RFC5733
        /// </summary>
        [TestMethod]
        [TestCategory("LocalCommand")]
        [DeploymentItem("TestData/ContactUpdateCommand1.xml")]
        public void TestContactUpdateCommand1()
        {
            Assert.Inconclusive("Not implemented");
        }

        /// <summary>
        /// Contact update response, example RFC5733
        /// </summary>
        [TestMethod]
        [TestCategory("LocalResponse")]
        [DeploymentItem("TestData/ContactUpdateResponse1.xml")]
        public void TestContactUpdateResponse1()
        {
            Assert.Inconclusive("Not implemented");
        }

        #endregion

        #region Contact Create

        /// <summary>
        /// Contact create command, example RFC5733
        /// NOTE: minor change to example XML, contact:disclose moved before contact:authinfo
        /// </summary>
        [TestMethod]
        [TestCategory("LocalCommand")]
        [DeploymentItem("TestData/ContactCreateCommand1.xml")]
        public void TestContactCreateCommand1()
        {
            string expected = File.ReadAllText("ContactCreateCommand1.xml");

            Contact contact = new Contact("sh8013", "John Doe", "Example Inc.",
                    "Dulles", "123 Example Dr.", "Suite 100", null, "VA", "20166-6503", "US",
                    "jdoe@example.com", new Telephone("+1.7035555555", "1234"),
                    new Telephone("+1.7035555556", null));
            contact.PostalInfo.m_type = PostalAddressType.INT;
            contact.DiscloseFlag = false;
            contact.DiscloseMask = Contact.DiscloseFlags.All & ~Contact.DiscloseFlags.Email & ~Contact.DiscloseFlags.Voice;

            var command = new ContactCreate(contact);
            command.TransactionId = "ABC-12345";
            command.Password = "2fooBAR";

            Assert.AreEqual(expected, command.ToXml().InnerXml);
        }

        /// <summary>
        /// Contact create response, example RFC5733
        /// </summary>
        [TestMethod]
        [TestCategory("LocalResponse")]
        [DeploymentItem("TestData/ContactCreateResponse1.xml")]
        public void TestContactCreateResponse1()
        {
            byte[] input = File.ReadAllBytes("ContactCreateResponse1.xml");
            var response = new ContactCreateResponse(input);

            Assert.AreEqual("1000", response.Code);
            Assert.AreEqual("Command completed successfully", response.Message);

            Assert.AreEqual("sh8013", response.ContactId);
            Assert.AreEqual("1999-04-03T22:00:00.0Z", response.DateCreated);

            Assert.AreEqual("ABC-12345", response.ClientTransactionId);
            Assert.AreEqual("54321-XYZ", response.ServerTransactionId);
        }

        #endregion

        #region TODO? Contact Transfer Query

        /// <summary>
        /// Contact transfer query command, example RFC5733
        /// </summary>
        [TestMethod]
        [TestCategory("LocalCommand")]
        [DeploymentItem("TestData/ContactTransferQueryCommand1.xml")]
        public void TestContactTransferQueryCommand1()
        {
            Assert.Inconclusive("Not implemented");

            /*
            string expected = File.ReadAllText("ContactTransferQueryCommand1.xml");

            var command = new ContactTransferQuery("sh8013");
            command.TransactionId = "ABC-12345";
            command.Password = "2fooBAR";

            Assert.AreEqual(expected, command.ToXml().InnerXml);
            */
        }

        /// <summary>
        /// Contact transfer query response, example RFC5733
        /// </summary>
        [TestMethod]
        [TestCategory("LocalResponse")]
        [DeploymentItem("TestData/ContactTransferQueryResponse1.xml")]
        public void TestContactTransferQueryResponse1()
        {
            Assert.Inconclusive("Not implemented");

            /*
            byte[] input = File.ReadAllBytes("ContactTransferQueryResponse1.xml");
            var response = new ContactTransferQueryResponse(input);

            Assert.AreEqual("1000", response.Code);
            Assert.AreEqual("Command completed successfully", response.Message);

            Assert.AreEqual("sh8013", response.ContactId);
            Assert.AreEqual("pending", response.TransferStatus); // trStatus

            Assert.AreEqual("ClientX", response.RequestingClient); //reID
            Assert.AreEqual("2000-06-06T22:00:00.0Z", response.RequestDate); //reDate

            Assert.AreEqual("ClientY", response.ActionClient); // acID
            Assert.AreEqual("2000-06-11T22:00:00.0Z", response.ActionDate); //acDate

            Assert.AreEqual("ABC-12345", response.ClientTransactionId);
            Assert.AreEqual("54322-XYZ", response.ServerTransactionId);
            */
        }

        #endregion

        #region TODO? Contact Transfer Request

        /// <summary>
        /// Contact transfer request command, example RFC5733
        /// </summary>
        [TestMethod]
        [TestCategory("LocalCommand")]
        [DeploymentItem("TestData/ContactTransferRequestCommand1.xml")]
        public void TestContactTransferRequestCommand1()
        {
            Assert.Inconclusive("Not implemented");

            /*
            string expected = File.ReadAllText("ContactTransferRequestCommand1.xml");

            var command = new ContactTransfer("sh8013");
            command.TransactionId = "ABC-12345";
            command.Password = "2fooBAR";

            Assert.AreEqual(expected, command.ToXml().InnerXml);
            */
        }

        /// <summary>
        /// Contact transfer command response, example RFC5733
        /// </summary>
        [TestMethod]
        [TestCategory("LocalResponse")]
        [DeploymentItem("TestData/ContactTransferRequestResponse1.xml")]
        public void TestContactTransferRequestResponse1()
        {
            Assert.Inconclusive("Not implemented");

            /*
            byte[] input = File.ReadAllBytes("ContactTransferRequestResponse1.xml");
            var response = new ContactTransferResponse(input);

            Assert.AreEqual("1001", response.Code);
            Assert.AreEqual("Command completed successfully; action pending", response.Message);

            Assert.AreEqual("sh8013", response.ContactId);
            Assert.AreEqual("pending", response.TransferStatus); // trStatus

            Assert.AreEqual("ClientX", response.RequestingClient); //reID
            Assert.AreEqual("2000-06-08T22:00:00.0Z", response.RequestDate); //reDate

            Assert.AreEqual("ClientY", response.ActionClient); // acID
            Assert.AreEqual("2000-06-13T22:00:00.0Z", response.ActionDate); //acDate

            Assert.AreEqual("ABC-12345", response.ClientTransactionId);
            Assert.AreEqual("54322-XYZ", response.ServerTransactionId);
            */
        }

        #endregion

        // Domain

        #region Domain Check
        
        /// <summary>
        /// Domain check command, example RFC5731
        /// </summary>
        [TestMethod]
        [TestCategory("LocalCommand")]
        [DeploymentItem("TestData/DomainCheckCommand1.xml")]
        public void TestDomainCheckCommand1()
        {
            string expected = File.ReadAllText("DomainCheckCommand1.xml");

            var command = new DomainCheck(
                    new List<string>()
                    {
                        "example.com",
                        "example.net",
                        "example.org"
                    }
            );
            command.TransactionId = "ABC-12345";

            Assert.AreEqual(expected, command.ToXml().InnerXml);
        }

        /// <summary>
        /// Domain check response, example RFC5733
        /// </summary>
        [TestMethod]
        [TestCategory("LocalResponse")]
        [DeploymentItem("TestData/DomainCheckResponse1.xml")]
        public void TestDomainCheckResponse1()
        {
            byte[] input = File.ReadAllBytes("DomainCheckResponse1.xml");
            var response = new DomainCheckResponse(input);

            Assert.AreEqual("1000", response.Code);
            Assert.AreEqual("Command completed successfully", response.Message);

            List<DomainCheckResult> expectedResults = new List<DomainCheckResult>()
                    {
                        new DomainCheckResult()
                        {
                            Name = "example.com",
                            Available = true,
                            Reason = null
                        },
                        new DomainCheckResult()
                        {
                            Name = "example.net",
                            Available = false,
                            Reason = "In use"
                        },
                        new DomainCheckResult()
                        {
                            Name = "example.org",
                            Available = true,
                            Reason = null
                        }
                    };

            CollectionAssert.AreEqual(expectedResults.ToArray(), response.Results.ToArray());

            Assert.AreEqual("ABC-12345", response.ClientTransactionId);
            Assert.AreEqual("54322-XYZ", response.ServerTransactionId);
        }
        
        #endregion

        #region Domain Release

        /// <summary>
        /// Domain release command, example RFC5731
        /// </summary>
        [TestMethod]
        [TestCategory("LocalCommand")]
        [DeploymentItem("TestData/DomainReleaseCommand1.xml")]
        public void TestDomainReleaseCommand1()
        {
            string expected = File.ReadAllText("DomainReleaseCommand1.xml");

            var command = new DomainRelease("epp-example.co.uk", "EXAMPLE-TAG");
            command.TransactionId = "ABC-12345";
            
            Assert.AreEqual(expected, command.ToXml().InnerXml);
        }

        /// <summary>
        /// Domain check response, example RFC5733
        /// </summary>
        [TestMethod]
        [TestCategory("LocalResponse")]
        [DeploymentItem("TestData/DomainReleaseResponse1.xml")]
        public void TestDomainReleaseResponse1()
        {
            byte[] input = File.ReadAllBytes("DomainReleaseResponse1.xml");
            var response = new DomainReleaseResponse(input);

            Assert.AreEqual("1001", response.Code);
            Assert.AreEqual("Command completed successfully; action pending", response.Message);

            Assert.AreEqual("ABC-12345", response.ClientTransactionId);
            Assert.AreEqual("54322-XYZ", response.ServerTransactionId);
        }


        #endregion

        #region TODO Domain Info

        /// <summary>
        /// Domain info command, example RFC5731
        /// </summary>
        [TestMethod]
        [TestCategory("LocalCommand")]
        [DeploymentItem("TestData/DomainInfoCommand1.xml")]
        public void TestDomainInfoCommand1()
        {
            Assert.Inconclusive("Not implemented");
        }

        /// <summary>
        /// Domain info response, example RFC5731
        /// </summary>
        [TestMethod]
        [TestCategory("LocalResponse")]
        [DeploymentItem("TestData/DomainInfoResponse1.xml")]
        public void TestDomainInfoResponse1()
        {
            Assert.Inconclusive("Not implemented");
        }

        #endregion

        #region TODO Domain Update

        /// <summary>
        /// Domain update command, example RFC5731
        /// </summary>
        [TestMethod]
        [TestCategory("LocalCommand")]
        [DeploymentItem("TestData/DomainUpdateCommand1.xml")]
        public void TestDomainUpdateCommand1()
        {
            Assert.Inconclusive("Not implemented");
        }

        /// <summary>
        /// Domain update response, example RFC5731
        /// </summary>
        [TestMethod]
        [TestCategory("LocalResponse")]
        [DeploymentItem("TestData/DomainUpdateResponse1.xml")]
        public void TestDomainUpdateResponse1()
        {
            Assert.Inconclusive("Not implemented");
        }

        #endregion

        #region TODO Domain Create

        /// <summary>
        /// Domain create command, example RFC5731
        /// </summary>
        [TestMethod]
        [TestCategory("LocalCommand")]
        [DeploymentItem("TestData/DomainCreateCommand1.xml")]
        public void TestDomainCreateCommand1()
        {
            Assert.Inconclusive("Not implemented");
        }

        /// <summary>
        /// Domain create response, example RFC5731
        /// </summary>
        [TestMethod]
        [TestCategory("LocalResponse")]
        [DeploymentItem("TestData/DomainCreateResponse1.xml")]
        public void TestDomainCreateResponse1()
        {
            Assert.Inconclusive("Not implemented");
        }

        #endregion

        #region TODO Domain Delete

        /// <summary>
        /// Domain delete command, example RFC5731
        /// </summary>
        [TestMethod]
        [TestCategory("LocalCommand")]
        [DeploymentItem("TestData/DomainDeleteCommand1.xml")]
        public void TestDomainDeleteCommand1()
        {
            Assert.Inconclusive("Not implemented");
        }

        /// <summary>
        /// Domain delete response, example RFC5731
        /// </summary>
        [TestMethod]
        [TestCategory("LocalResponse")]
        [DeploymentItem("TestData/DomainDeleteResponse1.xml")]
        public void TestDomainDeleteResponse1()
        {
            Assert.Inconclusive("Not implemented");
        }

        #endregion

        #region TODO Domain Renew

        /// <summary>
        /// Domain renew command, example RFC5731
        /// </summary>
        [TestMethod]
        [TestCategory("LocalCommand")]
        [DeploymentItem("TestData/DomainRenewCommand1.xml")]
        public void TestDomainRenewCommand1()
        {
            string expected = File.ReadAllText("DomainRenewCommand1.xml");

            var command = new DomainRenew("example.com", new DateTime(2000, 4, 3).ToString("yyyy-MM-dd"), new DomainPeriod(5, "y"));
            command.TransactionId = "ABC-12345";
            Assert.AreEqual(expected, command.ToXml().InnerXml);
        }

        /// <summary>
        /// Domain renew response, example RFC5731
        /// </summary>
        [TestMethod]
        [TestCategory("LocalResponse")]
        [DeploymentItem("TestData/DomainRenewResponse1.xml")]
        public void TestDomainRenewResponse1()
        {
            Assert.Inconclusive("Not implemented");
        }

        #endregion

        #region TODO Domain Transfer Query

        /// <summary>
        /// Domain transfer query command, example RFC5731
        /// </summary>
        [TestMethod]
        [TestCategory("LocalCommand")]
        [DeploymentItem("TestData/DomainTransferQueryCommand1.xml")]
        public void TestDomainTransferQueryCommand1()
        {
            Assert.Inconclusive("Not implemented");
        }

        /// <summary>
        /// Domain transfer query response, example RFC5731
        /// </summary>
        [TestMethod]
        [TestCategory("LocalResponse")]
        [DeploymentItem("TestData/DomainTransferQueryResponse1.xml")]
        public void TestDomainTransferQueryResponse1()
        {
            Assert.Inconclusive("Not implemented");
        }

        #endregion

        #region TODO Domain Transfer Request

        /// <summary>
        /// Domain transfer request command, example RFC5731
        /// </summary>
        [TestMethod]
        [TestCategory("LocalCommand")]
        [DeploymentItem("TestData/DomainTransferRequestCommand1.xml")]
        public void TestDomainTransferRequestCommand1()
        {
            Assert.Inconclusive("Not implemented");
        }

        /// <summary>
        /// Domain transfer request response, example RFC5731
        /// </summary>
        [TestMethod]
        [TestCategory("LocalResponse")]
        [DeploymentItem("TestData/DomainTransferRequestResponse1.xml")]
        public void TestDomainTransferRequestResponse1()
        {
            Assert.Inconclusive("Not implemented");
        }

        #endregion

        #region Poll

        /// <summary>
        /// Poll request command
        /// </summary>
        [TestMethod]
        [TestCategory("LocalCommand")]
        [DeploymentItem("TestData/PollRequestCommand1.xml")]
        public void TestPollReqCommand1()
        {
            string expected = File.ReadAllText("PollRequestCommand1.xml");

            var command = new Poll { Type = "req" };
            command.TransactionId = "ABC-12345";

            Assert.AreEqual(expected, command.ToXml().InnerXml);
        }

        /// <summary>
        /// Poll acknowledge command
        /// </summary>
        [TestMethod]
        [TestCategory("LocalCommand")]
        [DeploymentItem("TestData/PollAckCommand1.xml")]
        public void TestPollAckCommand1()
        {
            string expected = File.ReadAllText("PollAckCommand1.xml");

            var command = new Poll { MessageId = "12345", Type = "ack" };
            command.TransactionId = "ABC-12345";

            Assert.AreEqual(expected, command.ToXml().InnerXml);
        }

        /// <summary>
        /// Poll response, no messages
        /// </summary>
        [TestMethod]
        [TestCategory("LocalResponse")]
        [DeploymentItem("TestData/PollNoMsgsResponse1.xml")]
        public void TestPollNoMsgsResponse1()
        {
            byte[] input = File.ReadAllBytes("PollNoMsgsResponse1.xml");
            var response = new PollResponse(input);

            Assert.AreEqual("1300", response.Code);
            Assert.AreEqual("Command completed successfully; no messages", response.Message);

            Assert.AreEqual("ABC-12345", response.ClientTransactionId);
            Assert.AreEqual("54322-XYZ", response.ServerTransactionId);
        }

        /// <summary>
        /// Poll response, Contact deleted notification messages
        /// </summary>
        [TestMethod]
        [TestCategory("LocalResponse")]
        [DeploymentItem("TestData/PollMsgsResponse1.xml")]
        public void TestPollMsgsResponse1()
        {
            byte[] input = File.ReadAllBytes("PollMsgsResponse1.xml");
            var response = new PollResponse(input);

            Assert.AreEqual("1301", response.Code);
            Assert.AreEqual("Command completed successfully; ack to dequeue", response.Message);

            Assert.AreEqual("Contact deleted notification", response.Body);

            Assert.AreEqual("ABC-12345", response.ClientTransactionId);
            Assert.AreEqual("54322-XYZ", response.ServerTransactionId);
        }
        
        /// <summary>
        /// Poll response, Contact deleted notification messages
        /// </summary>
        [TestMethod]
        [TestCategory("LocalResponse")]
        [DeploymentItem("TestData/PollMsgsResponse2.xml")]
        public void TestPollMsgsResponse2()
        {
            byte[] input = File.ReadAllBytes("PollMsgsResponse2.xml");
            var response = new PollResponse(input);

            Assert.AreEqual("1301", response.Code);
            Assert.AreEqual("Command completed successfully; ack to dequeue", response.Message);

            Assert.AreEqual("Domains Released Notification", response.Body);

            Assert.AreEqual("ABC-12345", response.ClientTransactionId);
            Assert.AreEqual("54322-XYZ", response.ServerTransactionId);

            var notification = new DomainsReleasedNotification(File.ReadAllText("PollMsgsResponse2.xml"));

            Assert.IsNotNull(notification.DomainsReleased);
        }

        #endregion
    }
}
