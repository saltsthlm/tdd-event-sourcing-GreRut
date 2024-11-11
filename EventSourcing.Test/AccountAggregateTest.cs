using EventSourcing.Exceptions;
using EventSourcing.Models;
using EventSourcing.Test.Utilities;
using static EventSourcing.Events.Event;

namespace EventSourcing.Test;

public class AccountAggregateTEst
{
  [Fact]
  public async void Should_ReturnNull_FromEmptyStream()
  {
    // Arrange
    var events = await FileReader.GetStream(0);

    // Act
    var result = AccountAggregate.GenerateAggregate(events);

    // Assert
    result.Should().BeNull();
  }

  [Fact]
  public async void Should_Throw_FromUnspportedEvent()
  {
    // Arrange
    var events = await FileReader.GetStream(2);

    // Act
    var action = () => AccountAggregate.GenerateAggregate(events);

    // Assert
    action.Should().Throw<Exception>()
      .WithMessage("162*");
  }

  [Fact]
  public async void StartingWith_AccountCreatedEvent_Should_CreateAnAccount()
  {
    // Arrange
    var events = await FileReader.GetStream(1);
    var expectedAccount = new TestAccountAggregate
    {
      AccountId = "ACC123456",
      Balance = 5000,
      Currency = CurrencyType.Usd,
      CustomerId = "CUST001",
    };

    // Act
    var result = AccountAggregate.GenerateAggregate(events);

    // Assert
    result.Should().BeEquivalentTo(expectedAccount);
  }

  [Fact]
  public async void DepositEvent_Should_IncreaseBalance()
  {
    // Arrange
    var events = await FileReader.GetStream(3);
    var expectedAccount = new TestAccountAggregate
    {
      AccountId = "ACC123456",
      Balance = 5500,
      Currency = CurrencyType.Usd,
      CustomerId = "CUST001",
    };

    // Act
    var result = AccountAggregate.GenerateAggregate(events);

    // Assert
    result.Should().BeEquivalentTo(expectedAccount);
  }

  [Fact]
  public async void DepositEvents_Should_Chain()
  {
    // Arrange
    var events = await FileReader.GetStream(4);
    var expectedAccount = new TestAccountAggregate
    {
      AccountId = "ACC123456",
      Balance = 5700,
      Currency = CurrencyType.Usd,
      CustomerId = "CUST001",
    };

    // Act
    var result = AccountAggregate.GenerateAggregate(events);

    // Assert
    result.Should().BeEquivalentTo(expectedAccount);
  }

  [Fact]
  public async void DepositEvent_Should_Throw_IfDoneBeforeAccountCreatedEvent()
  {
    // Arrange
    var events = await FileReader.GetStream(5);

    // Act
    var action = () => AccountAggregate.GenerateAggregate(events);

    // Assert
    action.Should().Throw<Exception>()
      .WithMessage("128*");
  }

  [Fact]
  public async void DepositEvent_Should_Throw_IfAmountGoesAboveMaxBalance()
  {
    // Arrange
    var events = await FileReader.GetStream(6);

    // Act
    var action = () => AccountAggregate.GenerateAggregate(events);

    // Assert
    action.Should().Throw<Exception>()
      .WithMessage("281*");
  }

  [Fact]
  public async void WithdrawalEvent_Should_DepositMonay()
  {
    // Arrange
    var events = await FileReader.GetStream(7);
    var expectedAccount = new TestAccountAggregate
    {
      AccountId = "ACC123456",
      Balance = 4500,
      Currency = CurrencyType.Usd,
      CustomerId = "CUST001",
    };

    // Act
    var result = AccountAggregate.GenerateAggregate(events);

    // Assert
    result.Should().BeEquivalentTo(expectedAccount);
  }

  [Fact]
  public async void WithdrawalEvent_Should_Chain()
  {
    // Arrange
    var events = await FileReader.GetStream(8);
    var expectedAccount = new TestAccountAggregate
    {
      AccountId = "ACC123456",
      Balance = 4300,
      Currency = CurrencyType.Usd,
      CustomerId = "CUST001",
    };

    // Act
    var result = AccountAggregate.GenerateAggregate(events);

    // Assert
    result.Should().BeEquivalentTo(expectedAccount);
  }

  [Fact]
  public async void WithdrawalEvent_Should_Throw_IfDoneBeforeAccountCreatedEvent()
  {
    // Arrange
    var events = await FileReader.GetStream(9);

    // Act
    var action = () => AccountAggregate.GenerateAggregate(events);

    // Assert
    action.Should().Throw<Exception>()
      .WithMessage("128*");
  }

  [Fact]
  public async void WithdrawalEvent_Should_Chain_TogetherWithDepositEvent()
  {
    // Arrange
    var events = await FileReader.GetStream(10);
    var expectedAccount = new TestAccountAggregate
    {
      AccountId = "ACC123456",
      Balance = 4600,
      Currency = CurrencyType.Usd,
      CustomerId = "CUST001",
    };

    // Act
    var result = AccountAggregate.GenerateAggregate(events);

    // Assert
    result.Should().BeEquivalentTo(expectedAccount);
  }

  [Fact]
  public async void WithdrawalEvent_Should_Throw_IfBalanceWouldEndUpNegative()
  {
    // Arrange
    var events = await FileReader.GetStream(11);

    // Act
    var action = () => AccountAggregate.GenerateAggregate(events);

    // Assert
    action.Should().Throw<Exception>()
      .WithMessage("285*");
  }

  [Fact]
  public async void DeactivateEvent_Should_DeactivateAccount()
  {
    // Arrange
    var events = await FileReader.GetStream(12);
    var expectedAccount = new TestAccountAggregate
    {
      AccountId = "ACC123456",
      Balance = 5000,
      Currency = CurrencyType.Usd,
      CustomerId = "CUST001",
      Status = AccountStatus.Disabled,
    };

    // Act
    var result = AccountAggregate.GenerateAggregate(events);

    // Assert
    result.Should().BeEquivalentTo(expectedAccount);
  }

  [Fact]
  public async void DeactivateEvent_Should_AddEventToAccountLog()
  {
    // Arrange
    var events = await FileReader.GetStream(12);
    var expectedAccount = new TestAccountAggregate
    {
      AccountId = "ACC123456",
      Balance = 5000,
      Currency = CurrencyType.Usd,
      CustomerId = "CUST001",
      Status = AccountStatus.Disabled,
      AccountLog = [
        new (
          Type: "DEACTIVATE",
          Message: "Account inactive for 270 days",
          Timestamp: DateTime.Parse("2024-10-02T10:30:00")
        ),
      ]
    };

    // Act
    var result = AccountAggregate.GenerateAggregate(events);

    // Assert
    result.Should().BeEquivalentTo(expectedAccount);
  }

  [Fact]
  public async void Should_Add_Aditional_DeactivateEvent_ToAccountLog()
  {
    // Arrange
    var events = await FileReader.GetStream(13);
    var expectedAccount = new TestAccountAggregate
    {
      AccountId = "ACC123456",
      Balance = 5000,
      Currency = CurrencyType.Usd,
      CustomerId = "CUST001",
      Status = AccountStatus.Disabled,
      AccountLog = [
        new (
          Type: "DEACTIVATE",
          Message: "Account inactive for 270 days",
          Timestamp: DateTime.Parse("2024-10-02T10:30:00")
        ),
        new (
          Type: "DEACTIVATE",
          Message: "Security alert: suspicious activity",
          Timestamp: DateTime.Parse("2024-10-03T10:30:00")
        ),
      ]
    };

    // Act
    var result = AccountAggregate.GenerateAggregate(events);

    // Assert
    result.Should().BeEquivalentTo(expectedAccount);
  }

  [Fact]
  public async void Should_Throw_IfDeposit_IsDoneOnStatusDeactivated()
  {
    // Arrange
    var events = await FileReader.GetStream(14);

    // Act
    var action = () => AccountAggregate.GenerateAggregate(events);

    // Assert
    action.Should().Throw<Exception>()
      .WithMessage("344*");
  }

  [Fact]
  public async void Should_Throw_IfWidthrawal_IsDoneOnStatusDeactivated()
  {
    // Arrange
    var events = await FileReader.GetStream(15);

    // Act
    var result = Assert.Throws<Exception>(
      () => AccountAggregate.GenerateAggregate(events)
    );

    // Assert
    result.Message.Should().StartWith("344");
  }

  [Fact]
  public async void ActivateEvent_Should_ActivateADeactivatedAccount()
  {
    // Arrange
    var events = await FileReader.GetStream(16);
    var expectedAccount = new TestAccountAggregate
    {
      AccountId = "ACC123456",
      Balance = 5000,
      Currency = CurrencyType.Usd,
      CustomerId = "CUST001",
      Status = AccountStatus.Enabled,
    };

    // Act
    var result = AccountAggregate.GenerateAggregate(events);

    // Assert
    result.Should().BeEquivalentTo(expectedAccount);
  }

  [Fact]
  public async void ActivateEvent_Should_AddEventToAccountLog()
  {
    // Arrange
    var events = await FileReader.GetStream(16);
    var expectedAccount = new TestAccountAggregate
    {
      AccountId = "ACC123456",
      Balance = 5000,
      Currency = CurrencyType.Usd,
      CustomerId = "CUST001",
      Status = AccountStatus.Enabled,
      AccountLog = [
        new (
          Type: "DEACTIVATE",
          Message: "Account inactive for 270 days",
          Timestamp: DateTime.Parse("2024-10-02T10:30:00")
        ),
        new (
          Type: "ACTIVATE",
          Message: "Account reactivated",
          Timestamp: DateTime.Parse("2024-10-03T10:30:00")
        ),
      ]
    };

    // Act
    var result = AccountAggregate.GenerateAggregate(events);

    // Assert
    result.Should().BeEquivalentTo(expectedAccount);
  }

  [Fact]
  public async void ActivateEvent_ShouldNot_AddEventToAccountLog_IfAccountIsAlreadyActive()
  {
    // Arrange
    var events = await FileReader.GetStream(17);
    var expectedAccount = new TestAccountAggregate
    {
      AccountId = "ACC123456",
      Balance = 5000,
      Currency = CurrencyType.Usd,
      CustomerId = "CUST001",
      Status = AccountStatus.Enabled,
      AccountLog = [
        new (
          Type: "DEACTIVATE",
          Message: "Account inactive for 270 days",
          Timestamp: DateTime.Parse("2024-10-02T10:30:00")
        ),
        new (
          Type: "ACTIVATE",
          Message: "Account reactivated",
          Timestamp: DateTime.Parse("2024-10-03T10:30:00")
        ),
      ]
    };

    // Act
    var result = AccountAggregate.GenerateAggregate(events);

    // Assert
    result.Should().BeEquivalentTo(expectedAccount);
  }

  [Fact]
  public async void ClosureEvent_Should_CloseAccount()
  {
    // Arrange
    var events = await FileReader.GetStream(18);
    var expectedAccount = new TestAccountAggregate
    {
      AccountId = "ACC123456",
      Balance = 5000,
      Currency = CurrencyType.Usd,
      CustomerId = "CUST001",
      Status = AccountStatus.Closed,
    };

    // Act
    var result = AccountAggregate.GenerateAggregate(events);

    // Assert
    result.Should().BeEquivalentTo(expectedAccount);
  }

  [Fact]
  public async void ClosureEvent_Should_AddEventToAccountLog()
  {
    // Arrange
    var events = await FileReader.GetStream(18);
    var expectedAccount = new TestAccountAggregate
    {
      AccountId = "ACC123456",
      Balance = 5000,
      Currency = CurrencyType.Usd,
      CustomerId = "CUST001",
      Status = AccountStatus.Disabled,
      AccountLog = [
        new (
          Type: "CLOSURE",
          Message: "Reason: Customer request, Closing Balance: '5000'",
          Timestamp: DateTime.Parse("2024-10-02T10:30:00")
        ),
      ]
    };

    // Act
    var result = AccountAggregate.GenerateAggregate(events);

    // Assert
    result.Should().BeEquivalentTo(expectedAccount);
  }

  [Fact]
  public async void Events_Should_Throw_IfAppledOnClosedAccount()
  {
    // Arrange
    var events = await FileReader.GetStream(19);

    // Act
    var action = () => AccountAggregate.GenerateAggregate(events);

    // Assert
    action.Should().Throw<Exception>()
      .WithMessage("502*");
  }

  [Fact]
  public async void CurrencyChangeEvent_Should_ChangeAccountCurrency()
  {
    // Arrange
    var events = await FileReader.GetStream(20);
    var expectedAccount = new TestAccountAggregate
    {
      AccountId = "ACC123456",
      Balance = 51000,
      Currency = CurrencyType.Sek,
      CustomerId = "CUST001",
      Status = AccountStatus.Disabled,
    };

    // Act
    var result = AccountAggregate.GenerateAggregate(events);

    // Assert
    result.Should().Be(expectedAccount);
  }

  [Fact]
  public async void CurrencyChangeEvent_Should_AddEventToAccountLog()
  {
    // Arrange
    var events = await FileReader.GetStream(20);
    var expectedAccount = new TestAccountAggregate
    {
      AccountId = "ACC123456",
      Balance = 51000,
      Currency = CurrencyType.Sek,
      CustomerId = "CUST001",
      Status = AccountStatus.Disabled,
      AccountLog = [
        new (
          Type: "CURRENCY-CHANGE",
          Message: "Change currency from 'USD' to 'SEK'",
          Timestamp: DateTime.Parse("2024-10-02T10:30:00")
        ),
      ]
    };

    // Act
    var result = AccountAggregate.GenerateAggregate(events);

    // Assert
    result.Should().BeEquivalentTo(expectedAccount);
  }

  [Fact]
  public async void Should_Throw_IfEventIsMissingFromStream()
  {
    // Arrange
    var events = await FileReader.GetStream(21);

    // Act
    var action = () => AccountAggregate.GenerateAggregate(events);

    // Assert
    action.Should().Throw<Exception>()
      .WithMessage("511*");
  }
}
