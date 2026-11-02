-- Existing orders retain NULL because their payment method was not recorded.
ALTER TABLE orders ADD COLUMN IF NOT EXISTS payment_method VARCHAR(4)
    CHECK (payment_method IN ('Cash', 'Card'));
