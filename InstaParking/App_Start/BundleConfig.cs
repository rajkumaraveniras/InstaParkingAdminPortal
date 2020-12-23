using BundleTransformer.Core.Bundles;
using BundleTransformer.Core.Resolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace InstaParking
{
    public class BundleConfig
    {
        private static IBundleOrderer nullOrderer;

        public static void RegisterBundles(BundleCollection bundles)

        {
            BundleResolver.Current = new CustomBundleResolver();
            var commonStyleBundle = new CustomStyleBundle("~/Bundle/sass");

            commonStyleBundle.Include("~/Content/main.scss");
            commonStyleBundle.Orderer = nullOrderer;
            bundles.Add(commonStyleBundle);

        }
    }
}