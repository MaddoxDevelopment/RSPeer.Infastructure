using System.Linq;
using Microsoft.EntityFrameworkCore;
using RSPeer.Domain.Entities;

namespace RSPeer.Persistence
{
	public class RsPeerContext : DbContext
	{
		public RsPeerContext(DbContextOptions options) : base(options)
		{
		}

		public DbSet<Script> Scripts { get; set; }

		public DbSet<ScriptAccess> ScriptAccess { get; set; }

		public DbSet<PendingScriptMap> PendingScripts { get; set; }

		public DbSet<ScriptContent> ScriptContents { get; set; }

		public DbSet<User> Users { get; set; }

		public DbSet<UserGroup> UserGroups { get; set; }

		public DbSet<Group> Groups { get; set; }

		public DbSet<BalanceChange> BalanceChanges { get; set; }

		public DbSet<RunescapeClient> RunescapeClients { get; set; }

		public DbSet<Order> Orders { get; set; }

		public DbSet<Item> Items { get; set; }
		
		public DbSet<Data> Data { get; set; }
		
		public DbSet<SiteConfig> SiteConfig { get; set; }
		
		public DbSet<ScripterInfo> ScripterInfo { get; set; }
		
		public DbSet<DiscordAccount> DiscordAccounts { get; set; }
		
		public DbSet<BinaryFile> Files { get; set; }
		
		public DbSet<UserJsonData> UserJsonData { get; set; }
		
		public DbSet<PrivateScriptAccess> PrivateScriptAccess { get; set; }

		public DbSet<PaypalWebHookSale> PaypalWebHooks { get; set; }
		
		public DbSet<ApiClient> ApiClients { get; set; }

		public DbSet<PricePerQuantity> PricePerQuantity { get; set; }
		
		public DbSet<UserLog> UserLogs { get; set; }
		
		public DbSet<RemoteMessage> Messages { get; set; }
		
		public DbSet<Launcher> Launchers { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Model.GetEntityTypes().ToList()
				.ForEach(t => t.SetTableName(t.GetTableName().ToLower()));

			builder.Model.GetEntityTypes()
				.SelectMany(e => e.GetProperties())
				.ToList()
				.ForEach(c => c.SetColumnName(c.GetColumnName().ToLower()));

			builder.Entity<UserGroup>()
				.HasKey(t => new { t.GroupId, t.UserId });
			
			builder.Entity<UserJsonData>()
				.HasKey(t => new { t.Key, t.UserId });

			builder.Entity<UserGroup>()
				.HasOne(bc => bc.User)
				.WithMany(b => b.UserGroups)
				.HasForeignKey(bc => bc.UserId);

			builder.Entity<UserGroup>()
				.HasOne(bc => bc.Group)
				.WithMany(c => c.UserGroups)
				.HasForeignKey(bc => bc.GroupId);

			builder
				.Entity<User>()
				.HasIndex(w => w.Username);

			builder.Entity<User>().HasOne(w => w.DiscordAccount)
				.WithOne(w => w.User);

			builder.Entity<User>()
				.HasIndex(w => w.Email);

			builder.Entity<Item>().HasIndex(w => w.Name);

			builder.Entity<ScriptAccess>().Property(w => w.UserId).IsRequired();
			builder.Entity<ScriptAccess>().Property(w => w.ScriptId).IsRequired();

			builder.Entity<RunescapeClient>().Property(w => w.UserId).IsRequired();
			builder.Entity<RunescapeClient>().Property(w => w.LastUpdate).IsRequired();
			builder.Entity<RunescapeClient>().Property(w => w.Hash).IsRequired();
			builder.Entity<RunescapeClient>().HasIndex(w => w.Hash);

			builder.Entity<Group>().Property(w => w.Name).IsRequired();
			builder.Entity<BalanceChange>().Property(w => w.UserId).IsRequired();
			builder.Entity<BalanceChange>().Property(w => w.Timestamp).IsRequired();
			builder.Entity<BalanceChange>().Property(w => w.OldBalance).IsRequired();
			builder.Entity<BalanceChange>().Property(w => w.NewBalance).IsRequired();

			builder.Entity<Item>().Property(w => w.Sku).IsRequired();
			builder.Entity<Item>().Property(w => w.Name).IsRequired();
			builder.Entity<Item>().Property(w => w.PaymentMethod).IsRequired();
			builder.Entity<Item>().Property(w => w.Price).IsRequired();

			builder.Entity<Order>().Property(w => w.Total).IsRequired();
			builder.Entity<Order>().Property(w => w.Timestamp).IsRequired();
			builder.Entity<Order>().Property(w => w.UserId).IsRequired();
			builder.Entity<Order>().Property(w => w.ItemId).IsRequired();

			builder.Entity<Order>()
				.HasOne(p => p.Item)
				.WithMany(b => b.Orders)
				.HasForeignKey(p => p.ItemId);

			builder.Entity<Script>()
				.HasOne(p => p.ScriptContent);

			builder.Entity<Script>()
				.HasIndex(p => new { p.Name, p.Status }).IsUnique();

			builder.Entity<Script>().Property(w => w.Name).IsRequired();
			builder.Entity<Script>().Property(w => w.Description).IsRequired();
			builder.Entity<Script>().Property(w => w.UserId).IsRequired();
			builder.Entity<Script>().Property(w => w.Status).IsRequired();
			builder.Entity<Script>().Property(w => w.Type).IsRequired();
			builder.Entity<Script>().Property(w => w.Version).IsRequired();
			builder.Entity<Script>().Property(w => w.Category).IsRequired();
			builder.Entity<Script>().Property(w => w.ForumThread).IsRequired();
			builder.Entity<Script>().Property(w => w.TotalUsers).IsRequired().HasDefaultValue(0);

			builder.Entity<ScripterInfo>().Property(w => w.UserId).IsRequired();
			builder.Entity<ScripterInfo>().Property(w => w.GitlabUserId).IsRequired();
			builder.Entity<ScripterInfo>().Property(w => w.DateAdded).IsRequired();
			builder.Entity<ScripterInfo>().Property(w => w.GitlabUsername).IsRequired();
			builder.Entity<ScripterInfo>().Property(w => w.GitlabGroupPath).IsRequired();
			builder.Entity<ScripterInfo>().Property(w => w.GitlabGroupId).IsRequired();
			builder.Entity<ScripterInfo>().HasIndex(w => w.UserId);
			builder.Entity<ScripterInfo>().HasIndex(w => w.GitlabUserId);

			builder.Entity<BinaryFile>()
				.HasIndex(p => new { p.Name, p.Version }).IsUnique();
			builder.Entity<BinaryFile>().Property(w => w.Name).IsRequired();
			builder.Entity<BinaryFile>().Property(w => w.File).IsRequired();

			builder.Entity<SiteConfig>().HasIndex(w => w.Key).IsUnique();

			builder.Entity<PrivateScriptAccess>().HasIndex(w => new { w.UserId, w.ScriptId });
			builder.Entity<PrivateScriptAccess>().Property(w => w.ScriptId).IsRequired();
			builder.Entity<PrivateScriptAccess>().Property(w => w.UserId).IsRequired();
			
			builder.Entity<ApiClient>().HasIndex(w => w.Key);
			builder.Entity<ApiClient>().Property(w => w.UserId).IsRequired();
			builder.Entity<ApiClient>().Property(w => w.Key).IsRequired();
			builder.Entity<ApiClient>().Property(w => w.Timestamp).IsRequired();

			builder.Entity<PricePerQuantity>().HasIndex(w => w.Sku);
			builder.Entity<PricePerQuantity>().HasIndex(w => new { ItemId = w.Sku, w.Quantity }).IsUnique();
			builder.Entity<PricePerQuantity>().Property(w => w.Price).IsRequired();
			builder.Entity<PricePerQuantity>().Property(w => w.Quantity).IsRequired();
			builder.Entity<PricePerQuantity>().Property(w => w.Sku).IsRequired();
			
			builder.Entity<UserLog>().HasIndex(w => w.UserId);
			builder.Entity<UserLog>().Property(w => w.UserId).IsRequired();
			builder.Entity<UserLog>().Property(w => w.Message).IsRequired();
			builder.Entity<UserLog>().Property(w => w.Type).IsRequired();
			builder.Entity<UserLog>().Property(w => w.Timestamp).IsRequired();

			builder.Entity<RemoteMessage>().HasIndex(w => new {w.UserId, w.Consumer});
			
			base.OnModelCreating(builder);
		}
	}
}