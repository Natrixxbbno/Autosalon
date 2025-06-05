-- Insert manufacturers if table is empty
INSERT INTO manufacturers (name, country)
SELECT 'BMW', 'Germany'
WHERE NOT EXISTS (SELECT 1 FROM manufacturers WHERE name = 'BMW');

INSERT INTO manufacturers (name, country)
SELECT 'Audi', 'Germany'
WHERE NOT EXISTS (SELECT 1 FROM manufacturers WHERE name = 'Audi');

-- Insert BMW cars if not exists
INSERT INTO cars (manufacturer_id, model, year, color, price, registration_number, purchase_date)
SELECT 
    m.id,
    'X5',
    2023,
    'Black',
    85000.00,
    'BMW-X5-001',
    CURRENT_DATE - INTERVAL '2 months'
FROM manufacturers m
WHERE m.name = 'BMW'
AND NOT EXISTS (SELECT 1 FROM cars WHERE registration_number = 'BMW-X5-001');

INSERT INTO cars (manufacturer_id, model, year, color, price, registration_number, purchase_date)
SELECT 
    m.id,
    'M3',
    2023,
    'Green',
    95000.00,
    'BMW-M3-001',
    CURRENT_DATE - INTERVAL '1 month'
FROM manufacturers m
WHERE m.name = 'BMW'
AND NOT EXISTS (SELECT 1 FROM cars WHERE registration_number = 'BMW-M3-001');

INSERT INTO cars (manufacturer_id, model, year, color, price, registration_number, purchase_date)
SELECT 
    m.id,
    '320i',
    2022,
    'White',
    45000.00,
    'BMW-320-001',
    CURRENT_DATE - INTERVAL '3 months'
FROM manufacturers m
WHERE m.name = 'BMW'
AND NOT EXISTS (SELECT 1 FROM cars WHERE registration_number = 'BMW-320-001');

-- Insert Audi cars if not exists
INSERT INTO cars (manufacturer_id, model, year, color, price, registration_number, purchase_date)
SELECT 
    m.id,
    'A6',
    2023,
    'Silver',
    75000.00,
    'AUDI-A6-001',
    CURRENT_DATE - INTERVAL '2 months'
FROM manufacturers m
WHERE m.name = 'Audi'
AND NOT EXISTS (SELECT 1 FROM cars WHERE registration_number = 'AUDI-A6-001');

INSERT INTO cars (manufacturer_id, model, year, color, price, registration_number, purchase_date)
SELECT 
    m.id,
    'Q7',
    2023,
    'Green',
    90000.00,
    'AUDI-Q7-001',
    CURRENT_DATE - INTERVAL '1 month'
FROM manufacturers m
WHERE m.name = 'Audi'
AND NOT EXISTS (SELECT 1 FROM cars WHERE registration_number = 'AUDI-Q7-001');

INSERT INTO cars (manufacturer_id, model, year, color, price, registration_number, purchase_date)
SELECT 
    m.id,
    'RS6',
    2023,
    'Blue',
    120000.00,
    'AUDI-RS6-001',
    CURRENT_DATE - INTERVAL '4 months'
FROM manufacturers m
WHERE m.name = 'Audi'
AND NOT EXISTS (SELECT 1 FROM cars WHERE registration_number = 'AUDI-RS6-001');

-- Insert sales history if not exists
INSERT INTO sales_history (car_id, sale_date, sale_price)
SELECT 
    c.id,
    CURRENT_DATE - INTERVAL '1 month',
    c.price * 1.1
FROM cars c
WHERE c.model = 'X5'
AND NOT EXISTS (
    SELECT 1 FROM sales_history sh 
    WHERE sh.car_id = c.id
);

INSERT INTO sales_history (car_id, sale_date, sale_price)
SELECT 
    c.id,
    CURRENT_DATE - INTERVAL '2 months',
    c.price * 1.15
FROM cars c
WHERE c.model = 'A6'
AND NOT EXISTS (
    SELECT 1 FROM sales_history sh 
    WHERE sh.car_id = c.id
); 