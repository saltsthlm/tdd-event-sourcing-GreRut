﻿using EventSourcing.Events;
using EventSourcing.Exceptions;
using EventSourcing.Models;

namespace EventSourcing;

public class AccountAggregate
{

  public string? AccountId { get; set; }
  public decimal Balance { get; set; }
  public CurrencyType Currency { get; set; }
  public string? CustomerId { get; set; }
  public AccountStatus Status { get; set; }
  public List<LogMessage>? AccountLog { get; set; }

  private AccountAggregate(){}

  public static AccountAggregate? GenerateAggregate(Event[] events)
  {
    if (events.Length == 0)
    {
      return null;
    }
    
    var account = new AccountAggregate();
    foreach (var accountEvent in events)
    {
      account.Apply(accountEvent);
    }

    return account;
  }

  private void Apply(Event accountEvent)
  {
    switch (accountEvent)
    {
      case AccountCreatedEvent accountCreated:
        Apply(accountCreated);
        break;
      case DepositEvent deposit:
        Apply(deposit);
        break;
      case WithdrawalEvent withdrawal:
        Apply(withdrawal);
        break;
      case DeactivationEvent deactivation:
        Apply(deactivation);
        break;
      case ActivationEvent activation:
        Apply(activation);
        break;
      case ClosureEvent closure:
        Apply(closure);
        break;
      default:
        throw new EventTypeNotSupportedException("162 ERROR_EVENT_NOT_SUPPORTED");
    }
  } 

  private void Apply(AccountCreatedEvent accountCreated)
  {
    AccountId = accountCreated.AccountId;
    Balance = accountCreated.InitialBalance;
    Currency = accountCreated.Currency;
    CustomerId = accountCreated.CustomerId;
  }

  private void Apply(DepositEvent deposit)
  {
    if (AccountId == null)
      throw new AccountNotCreatedException("128 ERROR_ACCOUNT_UNINSTANTIATED");
    if (Status == AccountStatus.Disabled)
      throw new AccountDisabledException("344 ERROR_TRANSACTION_REJECTED_ACCOUNT_DEACTIVATED");
    if (deposit.Amount > Balance)
      throw new MaxBalanceExceeded("281 ERROR_BALANCE_SUCCEED_MAX_BALANCE");
    else
      Balance += deposit.Amount;
  }

  private void Apply(WithdrawalEvent withdrawal)
    {
    if (AccountId == null)
      throw new AccountNotCreatedException("128 ERROR_ACCOUNT_UNINSTANTIATED");
    if (Status == AccountStatus.Disabled)
      throw new Exception("344 ERROR_TRANSACTION_REJECTED_ACCOUNT_DEACTIVATED");
    if (withdrawal.amount > Balance)
      throw new MaxBalanceExceeded("285 ERROR_BALANCE_IN_NEGATIVE");
    else
      Balance -= withdrawal.amount;
  }

  private void Apply(DeactivationEvent deactivation)
  {
    Status = AccountStatus.Disabled;
    if (AccountLog == null)
      AccountLog = new List<LogMessage>();
    var logMessage = new LogMessage("DEACTIVATE", deactivation.Reason.ToString(), deactivation.Timestamp);
    AccountLog.Add(logMessage);
 
  }

  private void Apply(ActivationEvent activation)
  {
    if (AccountLog == null)
      AccountLog = new List<LogMessage>();
    if (Status == AccountStatus.Disabled)
    {
      var logMessage = new LogMessage("ACTIVATE", "Account reactivated",activation.Timestamp);
      AccountLog.Add(logMessage);
      Status = AccountStatus.Enabled;
    }
  }

  private void Apply(CurrencyChangeEvent currencyChange)
  {
    throw new NotImplementedException();
  }

  private void Apply(ClosureEvent closure)
  {
    if (AccountLog == null)
      AccountLog = new List<LogMessage>();
    var logMessage = new LogMessage("CLOSURE", "Reason: Customer request, Closing Balance: '5000'", closure.Timestamp);
    AccountLog.Add(logMessage);
    Status = AccountStatus.Closed;
  }
}
