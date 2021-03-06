﻿using Prism.Mvvm;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MinimalEmailClient.Models
{
    public class Mailbox : BindableBase
    {
        private string accountName = string.Empty;
        public string AccountName
        {
            get { return this.accountName; }
            set { SetProperty(ref this.accountName, value); }
        }

        private string mailboxName = string.Empty;
        public string MailboxName
        {
            get { return this.mailboxName; }
            private set { SetProperty(ref this.mailboxName, value); }
        }

        private string directoryPath = string.Empty;
        public string DirectoryPath
        {
            get { return this.directoryPath; }
            set
            {
                SetProperty(ref this.directoryPath, value);
                SetMailboxName();
            }
        }

        private string pathSeparator = string.Empty;
        public string PathSeparator
        {
            get { return this.pathSeparator; }
            set
            {
                SetProperty(ref this.pathSeparator, value);
                SetMailboxName();
            }
        }

        private string flagString = string.Empty;
        public string FlagString
        {
            get { return this.flagString; }
            set
            {
                SetProperty(ref this.flagString, value);
                string[] flags = this.flagString.Split(' ');
                Flags.Clear();
                Flags.AddRange(flags);
            }
        }

        private int uidNext;
        public int UidNext
        {
            get { return this.uidNext; }
            set { SetProperty(ref this.uidNext, value); }
        }

        private int uidValidity;
        public int UidValidity
        {
            get { return this.uidValidity; }
            set { SetProperty(ref this.uidValidity, value); }
        }

        public List<string> Flags { get; set; }

        public Mailbox()
        {
            Flags = new List<string>();
        }

        public override string ToString()
        {
            return
                "Mailbox:\n" +
                "AccountName: " + AccountName + "\n" +
                "MailboxName: " + MailboxName + "\n" +
                "DirectoryPath: " + DirectoryPath + "\n" +
                "PathSeparator: " + PathSeparator + "\n" +
                "UidNext: " + UidNext + "\n" +
                "UidValidity: " + UidValidity + "\n" +
                "FlagString: " + FlagString + "\n";
        }

        private void SetMailboxName()
        {
            if (PathSeparator == string.Empty || DirectoryPath == string.Empty)
            {
                return;
            }

            string pattern = "[^" + PathSeparator + "]+$";
            Match match = Regex.Match(DirectoryPath, pattern);
            if (match.Success)
            {
                MailboxName = match.Value.ToString().Trim('"');
            }
            else
            {
                MailboxName = DirectoryPath.Trim('"');
            }
        }
    }

    public class CompareMailbox : IEqualityComparer<Mailbox>
    {
        public bool Equals(Mailbox x, Mailbox y)
        {
            if (x == null || y == null)
            {
                return false;
            }
            return (x.AccountName == y.AccountName) && (x.DirectoryPath == y.DirectoryPath) && (x.FlagString == y.FlagString) && (x.PathSeparator == y.PathSeparator);
        }

        public int GetHashCode(Mailbox obj)
        {
            return obj.AccountName.GetHashCode() ^ obj.DirectoryPath.GetHashCode() ^ obj.FlagString.GetHashCode() ^ obj.PathSeparator.GetHashCode();
        }
    }
}
