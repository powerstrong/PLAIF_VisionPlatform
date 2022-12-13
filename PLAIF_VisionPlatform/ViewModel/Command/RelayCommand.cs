using System;
using System.Windows.Input;

namespace PLAIF_VisionPlatform
{
    internal class RelayCommand<T> : ICommand
    {
        // 한번만?? 생성자에서만 값 할당 (!=const)
        readonly Action<T> _excute;
        // CanExecute 함수를 외부에서 위임받을 수 있도록 Predicate<T> 사용
        readonly Predicate<T> _canexecute;

        public RelayCommand(Action<T> excute, Predicate<T> canexecute)
        {
            _excute = excute;
            _canexecute = canexecute;
        }

        // canexecute에 위임할 수 없을 때도 생성 가능
        public RelayCommand(Action<T> action) : this(action, null)
        {

        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; } // 렉이 걸린듯한 느낌이 들 때는 AsyncRelayCommand를 누겟에서 찾아서 해보도록
        }

        public bool CanExecute(object? parameter)
        {
            return _canexecute == null ? true : _canexecute((T)parameter!);
        }

        public void Execute(object? parameter)
        {
            _excute((T)parameter!);
        }
    }
}
