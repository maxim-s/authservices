﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kentor.AuthServices.Configuration;
using FluentAssertions;
using System.IdentityModel.Metadata;
using System.Globalization;
using NSubstitute;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class KentorAuthServicesSectionTests
    {
        Uri organizationSectionUrl;
        string organizationSectionName, organizationSectionDisplayName;

        [TestInitialize]
        public void BackupOrganizationSection()
        {
            organizationSectionUrl = KentorAuthServicesSection.Current.Metadata.Organization.Url;
            organizationSectionName = KentorAuthServicesSection.Current.Metadata.Organization.Name;
            organizationSectionDisplayName = KentorAuthServicesSection.Current.Metadata.Organization.DisplayName;
        }

        [TestCleanup]
        public void RestoreOrganizationSection()
        {
            if(!KentorAuthServicesSection.Current.Metadata.Organization.IsReadOnly())
            {
                KentorAuthServicesSection.Current.Metadata.Organization.Url = organizationSectionUrl;
                KentorAuthServicesSection.Current.Metadata.Organization.Name = organizationSectionName;
                KentorAuthServicesSection.Current.Metadata.Organization.DisplayName = organizationSectionDisplayName;
                KentorAuthServicesSection.Current.Metadata.Organization.AllowConfigEdits(false);
            }
        }

        [TestMethod]
        public void KentorAuthServicesSection_Organization_LoadedFromConfig()
        {
            var subject = KentorAuthServicesSection.Current.Organization;

            Organization expected = new Organization();
            expected.DisplayNames.Add(new LocalizedName("displayName", CultureInfo.InvariantCulture));
            expected.Names.Add(new LocalizedName("name", CultureInfo.InvariantCulture));
            expected.Urls.Add(new LocalizedUri(new Uri("http://url.example.com"), CultureInfo.InvariantCulture));

            subject.ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void KentorAuthServicesSection_Organization_HandlesMissing()
        {
            // If the organization element is missing in the config file, it will
            // still be present in the read config (which is stupid) and the strings
            // will be empty and the Url null. So let's pretend that the element
            // is missing from the config file by setting those values.
            KentorAuthServicesSection.Current.Metadata.Organization.AllowConfigEdits(true);
            KentorAuthServicesSection.Current.Metadata.Organization.Url = null;
            KentorAuthServicesSection.Current.Metadata.Organization.Name = "";
            KentorAuthServicesSection.Current.Metadata.Organization.DisplayName = "";

            // Reset the cached organization instance to force a reevaluation.
            KentorAuthServicesSection.Current.organization = null;

            KentorAuthServicesSection.Current.Organization.Should().BeNull();
        }
    }
}
