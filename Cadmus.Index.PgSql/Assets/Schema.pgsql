-- --------------------------------------------------------
-- index 
-- --------------------------------------------------------
CREATE EXTENSION pg_trgm;

-- item
CREATE TABLE item (
	id bpchar(36) NOT NULL,
	title varchar(500) NOT NULL,
	description varchar(1000) NOT NULL,
	facet_id varchar(100) NOT NULL,
	group_id varchar(100) NULL,
	sort_key varchar(1000) NULL,
	flags int4 NOT NULL,
	time_created timestamptz NOT NULL,
	creator_id varchar(100) NULL,
	time_modified timestamptz NOT NULL,
	user_id varchar(100) NULL,
	CONSTRAINT item_pk PRIMARY KEY (id)
);
CREATE INDEX item_facet_id_idx ON public.item USING btree (facet_id);
CREATE INDEX item_flags_idx ON public.item USING btree (flags);
CREATE INDEX item_group_id_idx ON public.item USING btree (group_id);
CREATE INDEX item_sort_key_idx ON public.item USING btree (sort_key);
CREATE INDEX item_title_idx ON public.item USING btree (title);

-- pin
CREATE TABLE pin (
	id serial4 NOT NULL,
	item_id bpchar(36) NOT NULL,
	part_id bpchar(36) NOT NULL,
	part_type_id varchar(100) NOT NULL,
	part_role_id varchar(100) NULL,
	"name" varchar(100) NOT NULL,
	value varchar(500) NOT NULL,
	time_indexed timestamptz NOT NULL,
	CONSTRAINT pin_pk PRIMARY KEY (id)
);
CREATE INDEX pin_name_idx ON public.pin USING btree (name);
CREATE INDEX pin_part_role_id_idx ON public.pin USING btree (part_role_id);
CREATE INDEX pin_part_type_id_idx ON public.pin USING btree (part_type_id);
CREATE INDEX pin_value_idx ON public.pin USING btree (value);
-- pin foreign keys
ALTER TABLE public.pin ADD CONSTRAINT pin_fk FOREIGN KEY (item_id) REFERENCES item(id) ON DELETE CASCADE ON UPDATE CASCADE;
