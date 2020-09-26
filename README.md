# SimpleQueue

This is an example of a simple in-memory queuing system backed up by a persistent store built using the dotnet core 3.1 [Channels API](https://docs.microsoft.com/en-us/dotnet/api/system.threading.channels.channel-1?view=netcore-3.1).

The example contains a `Server` which acts as the queue as well as a `Producer` and `Consumer`. The system is designed so that multiple `Producers` and `Consumers` can run at one time and killing any part of the system should not take the whole thing down. You can run each as a stand alone and bring them up and down as needed and the system should keep working.

NOTE: This is by no means perfect, it's just some ideas about how this might be done.

## Setup

The only setup required is the database. The system is designed to be used with a local instance of MSSQL. To configure the connection string modify `SimpleQueue.Server/appsetting.Development.json` to point to your instance.

To run migrations, from within `SimpleQueue.Server` execute:

```bash
dotnet ef database update
```

You will need to seed the db with an initial queue, here is a SQL statement for this:

```sql
INSERT INTO [SimpleQueue].[dbo].[Queue] VALUES (NEWID(), GETDATE(), GETDATE(), 'Test Queue')
```

Finally, you can execute each project by running:

```bash
dotnet run
```
