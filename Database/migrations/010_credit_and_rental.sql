CREATE TABLE IF NOT EXISTS auto_contracts (
    id SERIAL PRIMARY KEY,
    kind VARCHAR(6) NOT NULL CHECK (kind IN ('Credit', 'Rental')),
    last_name VARCHAR(100) NOT NULL CHECK (length(trim(last_name)) > 0),
    first_name VARCHAR(100) NOT NULL CHECK (length(trim(first_name)) > 0),
    phone VARCHAR(15) NOT NULL CHECK (phone ~ '^[0-9]{9,15}$'),
    email VARCHAR(255) NOT NULL,
    car_id INTEGER NOT NULL REFERENCES cars(id),
    payment_method VARCHAR(4) NOT NULL CHECK (payment_method IN ('Cash', 'Card')),
    car_price NUMERIC(12,2) NOT NULL CHECK (car_price > 0),
    down_payment NUMERIC(12,2),
    annual_rate NUMERIC(5,2),
    months INTEGER,
    monthly_payment NUMERIC(12,2),
    total_cost NUMERIC(14,2) NOT NULL CHECK (total_cost > 0),
    installments_paid INTEGER NOT NULL DEFAULT 0,
    start_date DATE NOT NULL,
    end_date DATE,
    daily_rate NUMERIC(12,2),
    closed_at TIMESTAMP,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CHECK ((kind = 'Credit' AND down_payment IS NOT NULL AND down_payment >= 0 AND down_payment < car_price
        AND annual_rate IS NOT NULL AND annual_rate BETWEEN 0 AND 100
        AND months IS NOT NULL AND months BETWEEN 1 AND 120
        AND monthly_payment IS NOT NULL AND monthly_payment > 0
        AND installments_paid BETWEEN 0 AND months AND end_date IS NULL AND daily_rate IS NULL)
        OR (kind = 'Rental' AND end_date IS NOT NULL AND end_date > start_date
        AND daily_rate IS NOT NULL AND daily_rate > 0 AND down_payment IS NULL
        AND annual_rate IS NULL AND months IS NULL AND monthly_payment IS NULL AND installments_paid = 0))
);
CREATE UNIQUE INDEX IF NOT EXISTS auto_contracts_active_car
    ON auto_contracts(car_id) WHERE closed_at IS NULL;
