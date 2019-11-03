//
using System;
using Microsoft.AspNetCore.Identity;
//
using NSG.WebSrv.Domain.Entities;
//
namespace NSG.Integration.Helpers
{
    public static class TestData
    {
        //
        public static string User_Name = "Phil";
        public static string User_Email = @"Phil@any.net";
        public static string Password = @"p@ssW0rd";
        //
        public static ApplicationUser[] ProfileData = new[] {
            new ApplicationUser { Email = User_Email, UserName = User_Name, PasswordHash = "AQAAAAEAACcQAAAAEB4oAR8WhJGi5QVXpuONJ4z69YqF/69GlCz4TtjbQVLA4ef69x0iDq5GLTzvqM2vwQ==", SecurityStamp = "VFV7PXFFMU4VZF57I3T7A6TXVF4VAY2M", ConcurrencyStamp = "24240e95-400c-434e-b498-16542c90de13", CompanyId = 1, FirstName = "Phillip", LastName = "Huhn", FullName = "Phillip Huhn", UserNicName = User_Name },
            new ApplicationUser { Email = "author@any.net", UserName = "author", PasswordHash = "AQAAAAEAACcQAAAAEGG4L+8q4FXRLAhrLWuALpnyStwzaYOaVA6LjBUrHHqs3AreDKMm9DnRa3B4PM1DYg==", SecurityStamp = "LTCQ4W2NCVQRESG6ZWMC7IBMWDZNICK7", ConcurrencyStamp = "2dab2144-81e5-4b76-a09c-c3b6c37f0f3b", CompanyId = 1, FirstName = "Author", LastName = "Huhn", FullName = "Author Huhn", UserNicName = "Art"  }
        };
        // UserName	NormalizedUserName	Email	NormalizedEmail	EmailConfirmed	PasswordHash	SecurityStamp	ConcurrencyStamp	PhoneNumber	PhoneNumberConfirmed	TwoFactorEnabled	LockoutEnd	LockoutEnabled	AccessFailedCount	CompanyId	CreateDate	FirstName	FullName	LastName	UserNicName
        // Phil    PHIL Phil@any.net PHIL@ANY.NET	0	AQAAAAEAACcQAAAAEJU+RgCBPX2MOPuob8QDzBcfox11osgmhNLWPYzOiusIidRyfs/Fh5QL6cxJnjFqUw==	KSEBFLTQCLU7G2X2XYXS5LCPSP6V4VGT	24240e95-400c-434e-b498-16542c90de13 NULL	0	0	NULL	1	0	1	2019-07-10 13:52:09.5300810	Phillip Phillip Huhn Huhn    Phillip
        // Phillip PHILLIP Phil@any.com PHIL@ANY.COM	0	AQAAAAEAACcQAAAAEGG4L+8q4FXRLAhrLWuALpnyStwzaYOaVA6LjBUrHHqs3AreDKMm9DnRa3B4PM1DYg==	LTCQ4W2NCVQRESG6ZWMC7IBMWDZNICK7	2dab2144-81e5-4b76-a09c-c3b6c37f0f3b NULL	0	0	NULL	1	0	1	2019-07-10 15:05:09.5071160	Phil Phil Huhn Huhn    Phil
        //
    }
}
