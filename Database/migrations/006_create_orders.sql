CREATE TABLE IF NOT EXISTS orders (
    id SERIAL PRIMARY KEY,
    last_name VARCHAR(100) NOT NULL CHECK (length(trim(last_name)) > 0),
    first_name VARCHAR(100) NOT NULL CHECK (length(trim(first_name)) > 0),
    contact_number VARCHAR(15) NOT NULL CHECK (contact_number ~ '^[0-9]{9,15}$'),
    email VARCHAR(255) NOT NULL CHECK (length(trim(email)) > 0),
    car_id INTEGER NOT NULL REFERENCES cars(id),
    quantity INTEGER NOT NULL CHECK (quantity BETWEEN 1 AND 1000),
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);
