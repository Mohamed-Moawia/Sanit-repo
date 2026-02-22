-- Stationery Store Database Initialization Script
-- This script runs when the PostgreSQL container starts for the first time

-- Create extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm";  -- For text search

-- Set timezone to Cairo
SET timezone = 'Africa/Cairo';

-- Create Arabic collation if not exists (PostgreSQL 12+)
-- Note: This may require ICU support

-- Grant permissions
GRANT ALL PRIVILEGES ON DATABASE "StationeryStore" TO stationery_admin;

-- Log completion
DO $$
BEGIN
    RAISE NOTICE 'Database initialization completed successfully';
END $$;
