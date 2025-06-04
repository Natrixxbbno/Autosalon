-- Insert manufacturers
INSERT INTO manufacturers (name, country) VALUES
('BMW', 'Germany'),
('Audi', 'Germany');

-- Insert BMW cars
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
WHERE m.name = 'BMW';

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
WHERE m.name = 'BMW';

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
WHERE m.name = 'BMW';

-- Insert Audi cars
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
WHERE m.name = 'Audi';

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
WHERE m.name = 'Audi';

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
WHERE m.name = 'Audi';

-- Insert some sales history
INSERT INTO sales_history (car_id, sale_date, sale_price)
SELECT 
    c.id,
    CURRENT_DATE - INTERVAL '1 month',
    c.price * 1.1
FROM cars c
WHERE c.model = 'X5';

INSERT INTO sales_history (car_id, sale_date, sale_price)
SELECT 
    c.id,
    CURRENT_DATE - INTERVAL '2 months',
    c.price * 1.15
FROM cars c
WHERE c.model = 'A6'; 