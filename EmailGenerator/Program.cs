using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;

namespace EmailGenerator
{
    using CliParser = CommandLine.Parser;

    /// <summary>
    /// Program entry point container object.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Simple list of public email domains that are used to
        /// generate emails unless intentionally excluded.
        /// </summary>
        protected static string[] KnownPublicMailProviders = 
        {
            "gmail.com",
            "yahoo.com",
            "hotmail.com",
            "outlook.com",
            "icloud.com",
            "mail.com",
            "aol.com"
        };

        /// <summary>
        /// Listing of known and common top level domains.
        /// </summary>
        protected static string[] KnownTLDs =
        {
            "com",
            "org",
            "net",
            "io",
            "gov",
            "edu",
            "mil"
        };

        /// <summary>
        /// Sneaky little helper to generate random character
        /// strings for email address.
        /// </summary>
        public static string RandomSoup { get; set; }

        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="args">CLI args</param>
        /// <returns>status code</returns>
        static int Main(string[] args)
        {
            var parser = new CliParser((configuration) =>
            {
                configuration.CaseSensitive = false;
                configuration.HelpWriter = Console.Error;
            });

            var cmdOpts = default(CliArgs);
            parser.ParseArguments<CliArgs>(args).WithParsed(opts => cmdOpts = opts);

            if (cmdOpts == null)
                return -1;

            // after this, RandomSoup should be a rougly 1200 character string
            // which is more than enough for email address generation.
            RandomSoup = Enumerable.Range(0, 100)
                .Select(x => Path.GetRandomFileName().Replace(".", ""))
                .Aggregate((x,y) => x + y);

            var pubCount = 0;
            if (cmdOpts.ExcludePublicDomains && cmdOpts.ExcludeGeneratedDomains)
            {
                Console.WriteLine("ERROR: Flags force exclusion " +
                    "of all types of address. None were generated.");
                return 0;
            }

            if (cmdOpts.ExcludeGeneratedDomains)
                pubCount = cmdOpts.Count;
            else 
            {
                // create a randomized breakdown.
                pubCount = Convert.ToInt32(Math.Round(
                    cmdOpts.Count * ((double)(new Random().Next(3, 7) * 10) / 100)));
            }

            // get a listing of 200 randomly generated domains.
            var domains = Enumerable.Range(0, 200).Select(v => GenerateRandomDomainName()).ToList();

            var emailListing = new SortedSet<string>();

            // keep generating randomized emails until we have the
            // desired number of public domain emails.
            // NOTE: This is done in a loop to ensure we get the desired
            // count even in the case where some duplicated are generated.
            while (emailListing.Count < pubCount)
            {
                var email = string.Format("{0}@{1}",
                    GenerateRandomStringPart(new Random().Next(1, cmdOpts.MaxEmailPartLength)),
                    KnownPublicMailProviders[new Random().Next(0, KnownPublicMailProviders.Length - 1)]);
                emailListing.Add(email);
            }
            
            // now just keep populating generated email addresses
            // until we've reached the desired count.
            while(emailListing.Count < cmdOpts.Count)
            {
                var email = string.Format("{0}@{1}",
                    GenerateRandomStringPart(new Random().Next(1, cmdOpts.MaxDomainPartLength)),
                    domains[new Random().Next(0, domains.Count - 1)]);
                emailListing.Add(email);
            }

            // push results to desired output destination (console or file)
            DumpResults(cmdOpts, emailListing);

            return 0;
        }

        /// <summary>
        /// Dumps all generated emails to whatever destination
        /// is available based on config.
        /// </summary>
        protected static void DumpResults(CliArgs cmdOpts, IEnumerable<string> listing)
        {
            if (string.IsNullOrEmpty(cmdOpts.OutputFile))
            {
                listing.ToList().ForEach(Console.WriteLine);
                return;
            }

            using (var fs = File.Open(cmdOpts.OutputFile, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            using (var writer = new StreamWriter(fs))
            {
                listing.ToList().ForEach(writer.WriteLine);
                writer.Flush();
            }
            
            // END FUNCTION
        }

        /// <summary>
        /// Generates and returns a domain name to be used
        /// for a generated email.
        /// </summary>
        protected static string GenerateRandomStringPart(int length) =>
            $"{(char)(0x61 + new Random().Next(0, 26))}" + RandomSoup.Substring(
                new Random().Next(0, RandomSoup.Length - (length - 1)), (length - 1));
        
        /// <summary>
        /// Generates a randomized domain name with a known
        /// top level dommain suffix.
        /// </summary>
        public static string GenerateRandomDomainName() => string.Format("{0}.{1}", 
            GenerateRandomStringPart(15),
            KnownTLDs[new Random().Next(0, KnownTLDs.Length - 1)]);
        
        // END CLASS
    }

    /// <summary>
    /// Command line args passed into app. Controls
    /// address types, number generated and where
    /// they are output to,
    /// </summary>
    internal class CliArgs
    {
        [Option('c', "count", HelpText = "Number of email address to generate.", Default = 1)]
        public int Count { get; set; }

        /// <summary>
        /// Notes to exclude public email domains and relies fully on 
        /// a set of generated domain names.
        /// </summary>
        [Option('p', "noPublic", HelpText = "Email addresses should not be generated from public email domains.", Default = false)]
        public bool ExcludePublicDomains { get; set; }

        /// <summary>
        /// Prevents randomly generated domains and relies on the set 
        /// of public email domain names.
        /// </summary>
        [Option('g', "noGenerated", HelpText = "Email addresses should not be generated from public email domains.", Default = false)]
        public bool ExcludeGeneratedDomains { get; set; }

        /// <summary>
        /// Output file where data is saved. If not provided, emails
        /// are dumped to stdout.
        /// </summary>
        [Option('o', "output", HelpText = "File location to save resulting emails.")]
        public string OutputFile { get; set; }

        /// <summary>
        /// Allows customization of mailbox target part of the email address.
        /// </summary>
        [Option('e', "emailPartLength", Default = 15, 
            HelpText = "Maximum length of the user part of the email address.")]
        public int MaxEmailPartLength { get; set; }

        /// <summary>
        /// Allows customization of the max length of domain part of the
        /// email address.
        /// </summary>
        [Option('d', "domainPartLength", Default = 20, 
            HelpText = "Maximum length of the domain part (excluding of the email address.")]
        public int MaxDomainPartLength { get; set; }

        // END CLASS
    }

    // END NAMESPACE
}
