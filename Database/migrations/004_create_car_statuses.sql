DO $$
BEGIN
    -- Создание таблицы статусов автомобилей, если она не существует
    IF NOT EXISTS (SELECT FROM pg_tables WHERE schemaname = 'public' AND tablename = 'car_statuses') THEN
        CREATE TABLE car_statuses (
            id SERIAL PRIMARY KEY,
            name VARCHAR(50) NOT NULL,
            color VARCHAR(16) DEFAULT '#cccccc',
            created_at TIMESTAMP NOT NULL DEFAULT NOW()
        );
    END IF;

    -- Добавление стандартных статусов, если они еще не существуют
    INSERT INTO car_statuses (name, color)
    SELECT 'Available', '#27ae60'
    WHERE NOT EXISTS (SELECT 1 FROM car_statuses WHERE name = 'Available');

    INSERT INTO car_statuses (name, color)
    SELECT 'Sold', '#b2bec3'
    WHERE NOT EXISTS (SELECT 1 FROM car_statuses WHERE name = 'Sold');

    INSERT INTO car_statuses (name, color)
    SELECT 'Reserved', '#fdcb6e'
    WHERE NOT EXISTS (SELECT 1 FROM car_statuses WHERE name = 'Reserved');
END $$; 