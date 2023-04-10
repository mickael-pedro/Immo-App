CREATE DATABASE immo_db;

USE immo_db;

CREATE TABLE tenant (
	id				    SERIAL PRIMARY KEY,
	civility		    TEXT NOT NULL,
	last_name		    TEXT NOT NULL,
	first_name		    TEXT NOT NULL,
	email			    TEXT NOT NULL
);

CREATE TABLE apartment (
	id				    SERIAL PRIMARY KEY,
	address			    TEXT NOT NULL,
	address_complement  TEXT,
	city			    TEXT NOT NULL,
	zip_code			INT NOT NULL
);

CREATE TABLE rental_contract (
	id				    	SERIAL PRIMARY KEY,
	charges_price	    	REAL NOT NULL,
	rent_price				REAL NOT NULL,
	security_deposit_price	REAL NOT NULL,
	security_deposit_status	TEXT NOT NULL,
	tenant_balance			REAL NOT NULL,
	rental_status			TEXT NOT NULL,
	rental_active			BOOL NOT NULL,
	fk_tenant_id	    	INTEGER NOT NULL,
	fk_apartment_id	    	INTEGER NOT NULL,
	FOREIGN KEY (fk_tenant_id) REFERENCES tenant(id)
    	ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY (fk_apartment_id) REFERENCES apartment(id)
    	ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE tenant_balance (
	id				    	SERIAL PRIMARY KEY,
	amount		    		REAL NOT NULL,
	fk_rental_contract_id	INTEGER NOT NULL,
	FOREIGN KEY (fk_rental_contract_id) REFERENCES rental_contract(id)
    	ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE inventory_fixture (
	id				    	SERIAL PRIMARY KEY,
	date_inv				DATE NOT NULL,
	type			    	TEXT NOT NULL,
	notes			    	TEXT NOT NULL,
	fk_rental_contract_id	INTEGER NOT NULL,
	FOREIGN KEY (fk_rental_contract_id) REFERENCES rental_contract(id)
    	ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE invoice (
	id					    SERIAL PRIMARY KEY,
	date_invoice		    DATE NOT NULL,
	amount				    REAL NOT NULL,
	type			    	TEXT NOT NULL,
	status				    TEXT NOT NULL,
	fk_rental_contract_id	INTEGER NOT NULL,
	FOREIGN KEY (fk_rental_contract_id) REFERENCES rental_contract(id)
    	ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE payment (
	id					    SERIAL PRIMARY KEY,
	date_payment		    DATE NOT NULL,
	amount				    REAL NOT NULL,
	origin				    TEXT NOT NULL,
	fk_invoice_id			INTEGER NOT NULL,
	fk_rental_contract_id	INTEGER NOT NULL,
	FOREIGN KEY (fk_invoice_id) REFERENCES invoice(id)
    	ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY (fk_rental_contract_id) REFERENCES rental_contract(id)
    	ON DELETE CASCADE ON UPDATE CASCADE
);
