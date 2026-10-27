using LZ.Model;
using LZ.Model.Bank.Wallet;
using LZ.Model.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Bank.Service.Interface
{
    public interface IBankWalletService
    {
        string Acct(IOptions<BankWalletOption> option, Live_BankWallet model);

        string MyWallet(IOptions<BankWalletOption> option, Live_BankWallet model);

        BankWalletAcctQuery Query(IOptions<BankWalletOption> option, Model.Live_BankWallet wallet);

        BankWalletAcctClose SendSmsBankWalletLogout(IOptions<BankWalletOption> option, Model.Live_BankWallet wallet);

        BankWalletAcctCloseMsgCheck SendSmsBankWalletLogoutCheck(IOptions<BankWalletOption> option, Model.Live_BankWallet wallet);

        BankWalletFinStatusQuery QueryFinStatus(IOptions<BankWalletOption> option, Model.Live_BankWallet wallet);
    }
}
