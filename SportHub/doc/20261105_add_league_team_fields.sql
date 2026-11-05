-- PostgreSQL schema patch for League / Team extension fields.
-- NOTE: Platform mapping keys are stored in AliasArray as dictionary JSON.

DO $$
BEGIN
    IF to_regclass('"League"') IS NULL THEN
        RAISE EXCEPTION 'Table "League" not found.';
    END IF;

    IF to_regclass('"Team"') IS NULL THEN
        RAISE EXCEPTION 'Table "Team" not found.';
    END IF;

    ALTER TABLE "League"
        ADD COLUMN IF NOT EXISTS "Abbr" varchar(64),
        ADD COLUMN IF NOT EXISTS "CountryCode" varchar(16),
        ADD COLUMN IF NOT EXISTS "Region" varchar(64),
        ADD COLUMN IF NOT EXISTS "Tier" integer,
        ADD COLUMN IF NOT EXISTS "MetaJson" jsonb;

    IF NOT EXISTS (
        SELECT 1
        FROM information_schema.columns
        WHERE table_name = 'League' AND column_name = 'LeagueCode'
    ) THEN
        ALTER TABLE "League" ADD COLUMN "LeagueCode" bigint;
    END IF;

    IF EXISTS (
        SELECT 1
        FROM information_schema.columns
        WHERE table_name = 'League' AND column_name = 'LeagueCode'
          AND udt_name <> 'int8'
    ) THEN
        ALTER TABLE "League"
            ALTER COLUMN "LeagueCode" TYPE bigint
            USING CASE UPPER("LeagueCode"::text)
                WHEN 'LEAGUE' THEN 1
                WHEN 'CUP' THEN 2
                WHEN 'SUPER' THEN 3
                WHEN 'SUPERCUP' THEN 3
                WHEN 'INTERNATIONAL' THEN 4
                ELSE 0
            END;
    END IF;

    IF EXISTS (
        SELECT 1
        FROM information_schema.columns
        WHERE table_name = 'League' AND column_name = 'LeagueType'
    ) THEN
        UPDATE "League"
        SET "LeagueCode" = CASE UPPER("LeagueType")
            WHEN 'LEAGUE' THEN 1
            WHEN 'CUP' THEN 2
            WHEN 'SUPER' THEN 3
            WHEN 'SUPERCUP' THEN 3
            WHEN 'INTERNATIONAL' THEN 4
            ELSE COALESCE("LeagueCode", 0)
        END
        WHERE "LeagueType" IS NOT NULL;

        ALTER TABLE "League" DROP COLUMN IF EXISTS "LeagueType";
    END IF;

    ALTER TABLE "League" ALTER COLUMN "LeagueCode" SET DEFAULT 0;
    UPDATE "League" SET "LeagueCode" = 0 WHERE "LeagueCode" IS NULL;
    ALTER TABLE "League" ALTER COLUMN "LeagueCode" SET NOT NULL;

    ALTER TABLE "League"
        DROP COLUMN IF EXISTS "StartSeason",
        DROP COLUMN IF EXISTS "EndSeason",
        DROP COLUMN IF EXISTS "Source",
        DROP COLUMN IF EXISTS "SourceUrl";

    ALTER TABLE "Team"
        ADD COLUMN IF NOT EXISTS "Abbr" varchar(64),
        ADD COLUMN IF NOT EXISTS "ShortName" varchar(128),
        ADD COLUMN IF NOT EXISTS "EnglishName" varchar(256),
        ADD COLUMN IF NOT EXISTS "CountryCode" varchar(16),
        ADD COLUMN IF NOT EXISTS "City" varchar(256),
        ADD COLUMN IF NOT EXISTS "FoundedYear" integer,
        ADD COLUMN IF NOT EXISTS "Stadium" varchar(256),
        ADD COLUMN IF NOT EXISTS "MetaJson" jsonb;

    ALTER TABLE "Team"
        DROP COLUMN IF EXISTS "StartSeason",
        DROP COLUMN IF EXISTS "EndSeason",
        DROP COLUMN IF EXISTS "Source",
        DROP COLUMN IF EXISTS "SourceUrl";

    COMMENT ON COLUMN "League"."Abbr" IS '联赛缩写（如 EPL、UCL）';
    COMMENT ON COLUMN "League"."CountryCode" IS '国家代码（ISO/FIFA，可空）';
    COMMENT ON COLUMN "League"."LeagueCode" IS '联赛代码（int64）：0=None,1=LEAGUE,2=CUP,3=SUPER,4=INTERNATIONAL';
    COMMENT ON COLUMN "League"."Region" IS '区域/洲别（如 Europe、Asia）';
    COMMENT ON COLUMN "League"."Tier" IS '联赛层级（1=顶级联赛）';
    COMMENT ON COLUMN "League"."MetaJson" IS '扩展元数据（jsonb）';

    COMMENT ON COLUMN "Team"."Abbr" IS '球队缩写（如 PSG、MU）';
    COMMENT ON COLUMN "Team"."ShortName" IS '球队简称';
    COMMENT ON COLUMN "Team"."EnglishName" IS '球队英文名';
    COMMENT ON COLUMN "Team"."CountryCode" IS '国家代码（ISO/FIFA，可空）';
    COMMENT ON COLUMN "Team"."City" IS '所在城市';
    COMMENT ON COLUMN "Team"."FoundedYear" IS '成立年份';
    COMMENT ON COLUMN "Team"."Stadium" IS '主场/球场';
    COMMENT ON COLUMN "Team"."MetaJson" IS '扩展元数据（jsonb）';
END $$;

-- Optional indexes for lookups
DO $$
BEGIN
    IF to_regclass('"League"') IS NOT NULL THEN
        EXECUTE 'CREATE INDEX IF NOT EXISTS "IX_League_Abbr" ON "League" ("Abbr")';
        EXECUTE 'CREATE INDEX IF NOT EXISTS "IX_League_CountryCode" ON "League" ("CountryCode")';
        EXECUTE 'CREATE INDEX IF NOT EXISTS "IX_League_LeagueCode" ON "League" ("LeagueCode")';
        EXECUTE 'CREATE INDEX IF NOT EXISTS "IX_League_Tier" ON "League" ("Tier")';
    END IF;

    IF to_regclass('"Team"') IS NOT NULL THEN
        EXECUTE 'CREATE INDEX IF NOT EXISTS "IX_Team_Abbr" ON "Team" ("Abbr")';
        EXECUTE 'CREATE INDEX IF NOT EXISTS "IX_Team_CountryCode" ON "Team" ("CountryCode")';
        EXECUTE 'CREATE INDEX IF NOT EXISTS "IX_Team_FoundedYear" ON "Team" ("FoundedYear")';
    END IF;
END $$;
