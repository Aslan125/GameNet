

namespace GameNet.Common.CommandsBase
{
    public interface ICommand
    {

        void Execute();

    }


    public interface ICommand<TReceiver>: ICommand
    {
        /// <summary>
        /// TReceiver - объект, для которого предназначена команда.
        /// </summary>
        TReceiver receiver { get; set; }
        
    }

    


}
