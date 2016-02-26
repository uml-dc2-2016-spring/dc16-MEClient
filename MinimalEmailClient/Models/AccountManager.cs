﻿using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MinimalEmailClient.Models
{
    class AccountManager : BindableBase
    {
        public ObservableCollection<Account> Accounts;
        public string Error = string.Empty;
        public readonly int MaxAccountNameLength = 30;

        public AccountManager()
        {
            Accounts = new ObservableCollection<Account>();
            LoadAccounts();
        }

        // Loads all accounts from database.
        public void LoadAccounts()
        {
            DatabaseManager databaseManager = new DatabaseManager();
            List<Account> accounts = databaseManager.GetAccounts();

            Accounts.Clear();
            foreach (Account account in accounts)
            {
                Downloader downloader = new Downloader(account);
                List<Mailbox> mailboxes = downloader.GetMailboxes();
                foreach (Mailbox mailbox in mailboxes)
                {
                    // DisplayName is the directory name without its path string.
                    string pattern = "[^" + mailbox.PathSeparator + "]+$";
                    Regex regx = new Regex(pattern);
                    Match match = regx.Match(mailbox.FullPath);
                    mailbox.DisplayName = match.Value.ToString();
                    account.Mailboxes.Add(mailbox);
                }
                Accounts.Add(account);
            }
        }

        // Returns true if successfully added the account. False, otherwise.
        public bool AddAccount(Account account)
        {
            DatabaseManager dm = new DatabaseManager();
            bool success = dm.AddAccount(account);
            if (success)
            {
                Accounts.Add(account);
            }
            else
            {
                Error = dm.Error;
            }

            return success;
        }

        public List<Mailbox> GetMailboxes(string accountName)
        {
            List<Mailbox> mailboxes = new List<Mailbox>();

            foreach (Account account in Accounts)
            {
                if (account.AccountName == accountName)
                {
                    Downloader downloader = new Downloader(account);
                    mailboxes = downloader.GetMailboxes();
                    foreach (Mailbox mailbox in mailboxes)
                    {
                        string pattern = "[^" + mailbox.PathSeparator + "]+$";
                        Regex regx = new Regex(pattern);
                        Match match = regx.Match(mailbox.FullPath);
                        mailbox.DisplayName = match.Value.ToString();
                    }
                    break;
                }
            }

            // Update the database with the newly downloaded data.
            DatabaseManager dm = new DatabaseManager();
            dm.UpdateMailboxes(accountName, mailboxes);

            return mailboxes;
        }
    }
}
