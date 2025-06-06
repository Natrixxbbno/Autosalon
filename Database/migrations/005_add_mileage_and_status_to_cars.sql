-- Добавление поля пробега и статуса в таблицу cars
ALTER TABLE cars
    ADD COLUMN IF NOT EXISTS mileage INTEGER DEFAULT 0,
    ADD COLUMN IF NOT EXISTS status_id INTEGER DEFAULT 1;

-- Добавление внешнего ключа на статусы
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.table_constraints 
        WHERE constraint_name = 'fk_status' AND table_name = 'cars'
    ) THEN
        ALTER TABLE cars
            ADD CONSTRAINT fk_status FOREIGN KEY (status_id) REFERENCES car_statuses(id);
    END IF;
END $$; 