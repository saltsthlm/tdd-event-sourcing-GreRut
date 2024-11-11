[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/vmTVbDw3)
# TDD-Driven Event Sourcing

Test-Driven Development (TDD) focuses on solving tasks incrementally, one small step at a time. In this weekend assignment, you'll create a function to generate an aggregate from an event stream. Don't worry if you're unfamiliar with event streaming; we assume this might be new to you.

See [Event Sourcing 101](./event-sourcing-101.md) for a quick introduction. A quick note: avoid spending too much time reading about event streaming details online. This is a big subject but all you need to know for today is the very basics.

## Instructions

In this assignment, you will write functions for handling banking events to generate the status of a bank account. We have supplied a starting boilerplate for you in the [./EventSourcing/AccountAggregate.cs](./EventSourcing/AccountAggregate.cs) file and your mission is by using a TDD approach, step-by-step implement more and more functionality. The tests are here to guide you, and not to be changed at any point.

The first few tests should already pass, and you can continue from there. You’re finished once all the tests pass and you feel confident that the code reflects your best work. Remember to refactor!

## Info

Each test is using an array of events that can be found in the [./EventSourcing.Test/test-streams](./EventSourcing.Test/test-streams) folder. The supplied `FileReader` deserializes the .json file for you so that you have the array available as an array. Feel free to review the files once you arrive at a correlating test.

The tests are expecting a few certain errors. Here is the list of those errors:
```text
128 ERROR_ACCOUNT_UNINSTANTIATED
162 ERROR_EVENT_NOT_SUPPORTED
281 ERROR_BALANCE_SUCCEED_MAX_BALANCE
285 ERROR_BALANCE_IN_NEGATIVE
344 ERROR_TRANSACTION_REJECTED_ACCOUNT_DEACTIVATED
502 ERROR_ACCOUNT_CLOSED
511 ERROR_INVALID_EVENT_STREAM
```

## Tips

One way to keep your code clean is to use something called a [Guard Clause](https://deviq.com/design-patterns/guard-clause) 

## FAQ

I feel lost and overwhelmed!

> Remember to take it slow, and to solve the tests one at a time!

---

Good luck and Have Fun!
