using System;
using System.Collections.Generic;
using System.Linq;

namespace bbb_sharp.Entities
{
    // tbl_IEntity
    // tbl_slack_IEntity

    public interface IEntity
    {
    }

    public abstract class PersistedEntity : IEntity
    {
        public int Id { get; set; }
    }

    public class SlackEntity : PersistedEntity, IEntity
    {
        public string ResponseUrl { get; set; }
    }

    public class SlackController
    {
        public void Endpoint()
        {
            //Magie

            //return iets zodat de usrr snapt dat er wat gebeurt
        }
    }

    public interface ServiceResponder
    {
        Type IEntityType { get; }

        void Send(IEntity IEntity);
    }

    public abstract class BaseServiceResponder<TIEntity> : ServiceResponder
        where TIEntity : IEntity
    {
        public Type IEntityType => typeof(TIEntity);

        public abstract void Send(TIEntity IEntity);

        public void Send(IEntity IEntity)
        {
            Send((TIEntity)IEntity);
        }
    }

    public class SlackServiceResponder : BaseServiceResponder<SlackEntity>
    {
        public override void Send(SlackEntity IEntity)
        {
            throw new NotImplementedException();
        }
    }

    public class ServiceResponderResolver
    {
        private readonly Dictionary<Type, ServiceResponder> _responderLookup;

        public void RegisterResolver(ServiceResponder serviceResponder)
        {

        }

        public ServiceResponder Resolve(IEntity IEntity)
        {
            switch (IEntity)
            {
                case SlackEntity slackIEntity:
                    return new SlackServiceResponder();

                default:
                    throw new NotImplementedException();
            }
        }
    }

    public class CommandContext
    {
        public ServiceResponder ServiceResponder { get; }
        public Message Message { get; }
        public IEntity Entity { get; }

        public CommandContext(ServiceResponder serviceResponder, Message message, IEntity IEntity)
        {
            ServiceResponder = serviceResponder;
            Message = message;
            Entity = IEntity;
        }
    }

    public abstract class Command
    {
        public abstract string Trigger { get; }

        public virtual bool Match(string input)
        {
            return input.StartsWith(Trigger);
        }

        public abstract void Execute(CommandContext context);
    }

    public class BonusCommand : Command
    {
        public override string Trigger => "bonus";

        public override void Execute(CommandContext context)
        {
            // dingetjes
            //Console.WriteLine(context.IEntity.id) <-- waarom kan dit?
            context.ServiceResponder.Send(context.Entity);
        }
    }

    public class CommandProcessor
    {
        public CommandProcessor(ServiceResponderResolver responderResolver)
        {
            this.responderResolver = responderResolver;
        }

        private Command[] _commands = new[] { new BonusCommand() };
        private readonly ServiceResponderResolver responderResolver;

        public void HandleMessage(Message message, IEntity IEntity)
        {
            // persist 

            var context = new CommandContext(responderResolver.Resolve(IEntity), message, IEntity); //context.IEntity.id?


            var command = _commands.SingleOrDefault(cmd => cmd.Match(message.Text));


            command.Execute(context); //<-- tasks eventueel?
        }
    }

    public class LongRunningTask
    {
        public IEntity IEntity { get; set; }

        public bool IsValid()
        {
            return true;
        }
    }

    public class Job
    {
        private readonly ServiceResponderResolver _resolver;

        public Job(ServiceResponderResolver resolver)
        {
            this._resolver = resolver;
        }

        // elke 5 seconde
        public void ProcessRunningTasks()
        {
            LongRunningTask[] runningTasks = null; // = retrieveTasks()

            foreach (var task in runningTasks)
            {
                //blockchain valideer
                //validated -> process task
                /*
                val receipt = web3j.ethGetTransactionReceipt(transaction.hash).sendAsync().get()
                val bonuses = BBBCore.contract.getBonusGivenEvents(receipt.result)
                for(bonusGivenEvent in bonuses) {
                    send(bonus)
                }
             */
                if (!task.IsValid())
                    continue;

                var responder = _resolver.Resolve(task.IEntity);
                responder.Send(task.IEntity);
            }

            /*
            
            tasks = getAllNewTasks()

            for task in tasks {
                
                if(task.isValid()) {
                    result = task.result

                    sendResponse(task, result) <-- response naar specifieke IEntity..?
                    update(task)
                }



            }
             */
        }
    }

    public class Message
    {
        public string Sender { get; set; }
        public string Text { get; set; }
    }

}

// Controller -> Command 