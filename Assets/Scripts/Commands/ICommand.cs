// 注意:interface無法直接暴露到Inspector中
// 所以得另外寫一個類別來實作這個interface
public interface ICommand
{
    bool CanHandle(CommandContext context);
    void Handle(CommandContext context);
}