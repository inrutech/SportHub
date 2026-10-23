using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SportHub.Repository.Model;

public partial class DBContext : DbContext
{
    public DBContext(DbContextOptions<DBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<League> League { get; set; }

    public virtual DbSet<Order> Order { get; set; }

    public virtual DbSet<OrderLeg> OrderLeg { get; set; }

    public virtual DbSet<Team> Team { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<League>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("StandardLeagues_pkey");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AliasArray).HasColumnType("jsonb");
            entity.Property(e => e.Country).HasMaxLength(256);
            entity.Property(e => e.Name).HasMaxLength(256);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("BetOrders_pkey");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ActualPayout)
                .HasPrecision(19, 4)
                .HasComment("实际结算派彩金额");
            entity.Property(e => e.LegCount).HasComment("总关数");
            entity.Property(e => e.Odds)
                .HasPrecision(19, 4)
                .HasComment("理论总赔率（各腿赔率连乘）");
            entity.Property(e => e.OrderCode).HasComment("订单类型");
            entity.Property(e => e.OrderSettleCode).HasComment("订单结算状态");
            entity.Property(e => e.PotentialPayout)
                .HasPrecision(19, 4)
                .HasComment("最高理论派彩金额（本金 * 总赔率)");
            entity.Property(e => e.SourceNo)
                .HasMaxLength(256)
                .HasComment("源订单号");
            entity.Property(e => e.Stake)
                .HasPrecision(19, 4)
                .HasComment("下注总数");
        });

        modelBuilder.Entity<OrderLeg>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("BetLegs_pkey");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AwayTeamId).HasComment("客队团队编号");
            entity.Property(e => e.BetOdds)
                .HasPrecision(19, 4)
                .HasComment("该腿下单时的原始赔率");
            entity.Property(e => e.BetScore).HasComment("下注时的比赛即时比分");
            entity.Property(e => e.FullTimeScore).HasComment("比赛最终全场比分");
            entity.Property(e => e.HalfTimeScore).HasComment("比赛最终半场比分");
            entity.Property(e => e.HandicapValue)
                .HasPrecision(19, 4)
                .HasComment("盘口数值（如大小球 1.25, 1.5, 1.75 等；独赢为空）");
            entity.Property(e => e.HomeTeamId).HasComment("主队团队编号");
            entity.Property(e => e.LeagueId).HasComment("联盟编号");
            entity.Property(e => e.LegIndex).HasComment("串关中的第几场比赛序号");
            entity.Property(e => e.LegSettleCode).HasComment("结算状态（PENDING:待定, WON:全赢, HALF_WON:赢一半, PUSH:走水, HALF_LOST:输一半, LOST:全输）");
            entity.Property(e => e.MarketCode).HasComment("玩法类型（ASIAN_HANDICAP:让球, OVER_UNDER:大小球, MONEYLINE_1X2:独赢）");
            entity.Property(e => e.MatchStartTime).HasComment("比赛开始时间");
            entity.Property(e => e.PeriodCode).HasComment("比赛阶段（FULL_TIME:全场, HALF_TIME:上半场）");
            entity.Property(e => e.SelectionCode).HasComment("投注选项（如 Under, Over, HOME, AWAY, DRAW 等）");
            entity.Property(e => e.SettledOdds)
                .HasPrecision(19, 4)
                .HasComment("实际结算有效赔率（全赢为原始赔率，赢一半为折算赔率，走水为1.0等）");
            entity.Property(e => e.SourceCode).HasComment("源编号");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("StandardTeams_pkey");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AliasArray).HasColumnType("jsonb");
            entity.Property(e => e.Country).HasMaxLength(256);
            entity.Property(e => e.Name).HasMaxLength(256);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
