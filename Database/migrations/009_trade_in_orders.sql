ALTER TABLE orders
    ADD COLUMN IF NOT EXISTS trade_in_model VARCHAR(150),
    ADD COLUMN IF NOT EXISTS trade_in_manufacturer_id INTEGER REFERENCES manufacturers(id),
    ADD COLUMN IF NOT EXISTS trade_in_color VARCHAR(50),
    ADD COLUMN IF NOT EXISTS trade_in_registration VARCHAR(20),
    ADD COLUMN IF NOT EXISTS trade_in_year INTEGER,
    ADD COLUMN IF NOT EXISTS trade_in_mileage INTEGER,
    ADD COLUMN IF NOT EXISTS trade_in_value NUMERIC(12,2),
    ADD COLUMN IF NOT EXISTS desired_car_price NUMERIC(12,2),
    ADD COLUMN IF NOT EXISTS trade_in_balance NUMERIC(12,2)
        GENERATED ALWAYS AS (desired_car_price - trade_in_value) STORED;

DO $$ BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'orders_trade_in_valid') THEN
        ALTER TABLE orders ADD CONSTRAINT orders_trade_in_valid CHECK (
            (trade_in_model IS NULL AND trade_in_year IS NULL AND trade_in_mileage IS NULL
                AND trade_in_value IS NULL AND desired_car_price IS NULL
                AND trade_in_manufacturer_id IS NULL AND trade_in_color IS NULL AND trade_in_registration IS NULL)
            OR
            (trade_in_model IS NOT NULL AND length(trim(trade_in_model)) > 0
                AND trade_in_year IS NOT NULL AND trade_in_year >= 1900
                AND trade_in_mileage IS NOT NULL AND trade_in_mileage >= 0
                AND trade_in_value IS NOT NULL AND trade_in_value > 0
                AND desired_car_price IS NOT NULL AND desired_car_price > 0 AND quantity = 1)
                AND (trade_in_model IS NULL OR (trade_in_manufacturer_id IS NOT NULL
                    AND trade_in_color IS NOT NULL AND length(trim(trade_in_color)) > 0
                    AND trade_in_registration IS NOT NULL AND length(trim(trade_in_registration)) > 0))
        );
    END IF;
END $$;

CREATE UNIQUE INDEX IF NOT EXISTS orders_pending_trade_registration
    ON orders (upper(trim(trade_in_registration)))
    WHERE trade_in_model IS NOT NULL AND completed_at IS NULL;
