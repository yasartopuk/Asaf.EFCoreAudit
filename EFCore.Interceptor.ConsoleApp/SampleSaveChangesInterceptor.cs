using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Interceptor.ConsoleApp;

public class SampleSaveChangesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, 
        InterceptionResult<int> result)
    {
        // Eklenen kayıtlar alınıyor
        var entries = eventData.Context.ChangeTracker.Entries<IEntity>();

        foreach (var entry in entries.Where(e => e.State == EntityState.Added))
        {
            Console.WriteLine($"Added: {entry.Entity.GetType().Name}" + $" - {entry.Entity}");
            Console.WriteLine("------------------------");
        }

        // Değişen kayıtların önceki ve sonraki değerleri alınıyor
        foreach (var entry in entries.Where(e => e.State == EntityState.Modified))
        {
            Console.WriteLine($"Modified: {entry.Entity.GetType().Name}" + $" - Id: {entry.Entity.Id}");
            foreach (var property in entry.OriginalValues.Properties)
            {
                var original = entry.OriginalValues[property];
                var current = entry.CurrentValues[property];

                // Alanın değişmiş ise loglanoyor
                if (original?.ToString() == current?.ToString())
                    continue;

                Console.WriteLine($"{property.Name}: {original} > {current}");
            }
            Console.WriteLine("------------------------");
        }

        // silinen kayıtlar alınıyor
        foreach (var entry in entries.Where(e => e.State == EntityState.Deleted))
        {
            Console.WriteLine($"Deleted: {entry.Entity.GetType().Name} " +
                $"- Id: {entry.Entity.Id} ");
            Console.WriteLine("------------------------");

        }

        return base.SavingChanges(eventData, result);
    }
}
