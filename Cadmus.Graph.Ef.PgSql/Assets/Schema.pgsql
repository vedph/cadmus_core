-- namespace_lookup
CREATE TABLE namespace_lookup (
	id varchar(50) NOT NULL,
	uri varchar(500) NOT NULL,
	CONSTRAINT namespace_lookup_pk PRIMARY KEY (id)
);
CREATE INDEX namespace_lookup_uri_idx ON namespace_lookup USING btree (uri);

-- uri_lookup
CREATE TABLE uri_lookup (
	id serial4 NOT NULL,
	uri varchar(500) NOT NULL,
	CONSTRAINT uri_lookup_pk PRIMARY KEY (id)
);
CREATE INDEX uri_lookup_uri_idx ON uri_lookup USING btree (uri);

-- uid_lookup
CREATE TABLE uid_lookup (
	id serial4 NOT NULL,
	sid varchar(500) NOT NULL,
	unsuffixed varchar(500) NOT NULL,
	has_suffix boolean NOT NULL,
	CONSTRAINT uid_lookup_pk PRIMARY KEY (id)
);
CREATE INDEX uid_lookup_unsuffixed_idx ON uid_lookup USING btree (unsuffixed);

-- node
CREATE TABLE node (
	id int4 NOT NULL,
	is_class boolean NOT NULL,
	tag varchar(50) NULL,
	"label" varchar(500) NULL,
	source_type int4 NOT NULL,
	sid varchar(500) NULL,
	CONSTRAINT node_pk PRIMARY KEY (id)
);
-- node foreign keys
ALTER TABLE node ADD CONSTRAINT node_fk FOREIGN KEY (id) REFERENCES uri_lookup(id) ON DELETE CASCADE ON UPDATE CASCADE;

-- node_class
CREATE TABLE node_class (
	id serial4 NOT NULL,
	node_id int4 NOT NULL,
	class_id int4 NOT NULL,
	"level" int4 NOT NULL,
	CONSTRAINT node_class_pk PRIMARY KEY (id)
);
-- node_class foreign keys
ALTER TABLE node_class ADD CONSTRAINT node_class_fk FOREIGN KEY (node_id) REFERENCES node(id) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE node_class ADD CONSTRAINT node_class_fk_1 FOREIGN KEY (class_id) REFERENCES node(id) ON DELETE CASCADE ON UPDATE CASCADE;

-- property
CREATE TABLE property (
	id int4 NOT NULL,
	data_type varchar(100) NULL,
	lit_editor varchar(100) NULL,
	description varchar(5000) NULL,
	CONSTRAINT property_pk PRIMARY KEY (id)
);
-- property foreign keys
ALTER TABLE property ADD CONSTRAINT property_fk FOREIGN KEY (id) REFERENCES node(id) ON DELETE CASCADE ON UPDATE CASCADE;

-- triple
CREATE TABLE triple (
	id serial4 NOT NULL,
	s_id int4 NOT NULL,
	p_id int4 NOT NULL,
	o_id int4 NULL,
	o_lit varchar NULL,
	o_lit_type varchar(100) NULL,
	o_lit_lang varchar(10) NULL,
	o_lit_ix varchar(15000) NULL,
	o_lit_n float8 NULL,
	sid varchar(500) NULL,
	tag varchar(50) NULL,
	CONSTRAINT triple_pk PRIMARY KEY (id)
);
CREATE INDEX triple_o_lit_ix_idx ON triple USING btree (o_lit_ix);
CREATE INDEX triple_sid_idx ON triple USING btree (sid);
-- triple foreign keys
ALTER TABLE triple ADD CONSTRAINT triple_fk FOREIGN KEY (s_id) REFERENCES node(id) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE triple ADD CONSTRAINT triple_fk_1 FOREIGN KEY (p_id) REFERENCES node(id) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE triple ADD CONSTRAINT triple_fk_2 FOREIGN KEY (o_id) REFERENCES node(id) ON DELETE CASCADE ON UPDATE CASCADE;

-- mapping
CREATE TABLE "mapping" (
	id serial4 NOT NULL,
	ordinal int4 NOT NULL,
	"name" varchar(100) NOT NULL,
	source_type int4 NOT NULL,
	facet_filter varchar(100) NULL,
	group_filter varchar(100) NULL,
	title_filter varchar(100) NULL,
	flags_filter int4 NULL,
	part_type_filter varchar(100) NULL,
	part_role_filter varchar(100) NULL,
	description varchar(1000) NULL,
	"source" varchar(500) NOT NULL,
	sid varchar(500) NULL,
	scalar_pattern varchar(500) NULL,
	CONSTRAINT mapping_pk PRIMARY KEY (id)
);
CREATE INDEX mapping_facet_filter_idx ON mapping USING btree (facet_filter);
CREATE INDEX mapping_name_idx ON mapping USING btree (name);

-- mapping_link
CREATE TABLE mapping_link (
	parent_id int4 NOT NULL,
	child_id int4 NOT NULL,
	CONSTRAINT mapping_link_pk PRIMARY KEY (parent_id, child_id),
	CONSTRAINT mapping_link_fk_parent FOREIGN KEY (parent_id) REFERENCES "mapping"(id) ON DELETE CASCADE ON UPDATE CASCADE,
	CONSTRAINT mapping_link_fk_child FOREIGN KEY (child_id) REFERENCES "mapping"(id) ON DELETE CASCADE ON UPDATE CASCADE
);

-- mapping_out_node
CREATE TABLE mapping_out_node (
	id serial4 NOT NULL,
	mapping_id int4 NULL,
	ordinal int4 NOT NULL,
	"name" varchar(100) NOT NULL,
	uid varchar(1000) NOT NULL,
	"label" varchar(1000) NOT NULL,
	tag varchar(100) NULL,
	CONSTRAINT mapping_out_node_pk PRIMARY KEY (id)
);
-- mapping_out_node foreign keys
ALTER TABLE mapping_out_node ADD CONSTRAINT mapping_out_node_fk FOREIGN KEY (mapping_id) REFERENCES "mapping"(id) ON DELETE CASCADE ON UPDATE CASCADE;

-- mapping_out_triple
CREATE TABLE mapping_out_triple (
	id serial4 NOT NULL,
	mapping_id int4 NOT NULL,
	ordinal int4 NOT NULL,
	s varchar(1000) NOT NULL,
	p varchar(1000) NOT NULL,
	o varchar(1000) NULL,
	ol varchar(10000) NULL,
	CONSTRAINT mapping_out_triple_pk PRIMARY KEY (id)
);
-- mapping_out_triple foreign keys
ALTER TABLE mapping_out_triple ADD CONSTRAINT mapping_out_triple_fk FOREIGN KEY (mapping_id) REFERENCES "mapping"(id) ON DELETE CASCADE ON UPDATE CASCADE;

-- mapping_out_meta
CREATE TABLE mapping_out_meta (
	id serial4 NOT NULL,
	mapping_id int4 NOT NULL,
	ordinal int4 NOT NULL,
	"name" varchar(100) NOT NULL,
	value varchar(10000) NOT NULL,
	CONSTRAINT mapping_out_meta_pk PRIMARY KEY (id)
);
-- mapping_out_meta foreign keys
ALTER TABLE mapping_out_meta ADD CONSTRAINT mapping_out_meta_fk FOREIGN KEY (mapping_id) REFERENCES "mapping"(id) ON DELETE CASCADE ON UPDATE CASCADE;

-- --------------------------------------------------------
-- graph preset data
-- --------------------------------------------------------

-- core namespaces
INSERT INTO namespace_lookup(id, uri) VALUES('owl', 'http://www.w3.org/2002/07/owl#');
INSERT INTO namespace_lookup(id, uri) VALUES('rdf', 'http://www.w3.org/1999/02/22-rdf-syntax-ns#');
INSERT INTO namespace_lookup(id, uri) VALUES('rdfs', 'http://www.w3.org/2000/01/rdf-schema#');
INSERT INTO namespace_lookup(id, uri) VALUES('xml', 'http://www.w3.org/XML/1998/namespace');
INSERT INTO namespace_lookup(id, uri) VALUES('xsd', 'http://www.w3.org/2001/XMLSchema#');

-- core classes URIs
-- NOTE: if you modify any of the following, update the IDs for the nodes below
INSERT INTO uri_lookup(uri) VALUES('rdfs:Resource');
INSERT INTO uri_lookup(uri) VALUES('rdfs:Literal');
INSERT INTO uri_lookup(uri) VALUES('rdfs:XMLLiteral');
INSERT INTO uri_lookup(uri) VALUES('rdfs:Class');
INSERT INTO uri_lookup(uri) VALUES('rdfs:Property');
INSERT INTO uri_lookup(uri) VALUES('rdfs:DataType');
-- core classes nodes
INSERT INTO node(id, is_class, label, tag, source_type, sid) VALUES(1, true, 'rdfs:Resource', NULL, 0, NULL);
INSERT INTO node(id, is_class, label, tag, source_type, sid) VALUES(2, true, 'rdfs:Literal', NULL, 0, NULL);
INSERT INTO node(id, is_class, label, tag, source_type, sid) VALUES(3, true, 'rdfs:XMLLiteral', NULL, 0, NULL);
INSERT INTO node(id, is_class, label, tag, source_type, sid) VALUES(4, true, 'rdfs:Class', NULL, 0, NULL);
INSERT INTO node(id, is_class, label, tag, source_type, sid) VALUES(5, true, 'rdfs:Property', NULL, 0, NULL);
INSERT INTO node(id, is_class, label, tag, source_type, sid) VALUES(6, true, 'rdfs:DataType', NULL, 0, NULL);

-- core properties URIs
INSERT INTO uri_lookup(uri) VALUES('rdf:type');
INSERT INTO uri_lookup(uri) VALUES('rdfs:subClassOf');
INSERT INTO uri_lookup(uri) VALUES('rdfs:subPropertyOf');
INSERT INTO uri_lookup(uri) VALUES('rdfs:domain');
INSERT INTO uri_lookup(uri) VALUES('rdfs:range');
INSERT INTO uri_lookup(uri) VALUES('rdfs:label');
INSERT INTO uri_lookup(uri) VALUES('rdfs:comment');
INSERT INTO uri_lookup(uri) VALUES('rdfs:seeAlso');
INSERT INTO uri_lookup(uri) VALUES('rdfs:isDefinedBy');
-- core properties nodes
INSERT INTO node(id, is_class, label, tag, source_type, sid) VALUES(7, false, 'is-a', 'property', 0, NULL);
INSERT INTO node(id, is_class, label, tag, source_type, sid) VALUES(8, false, 'rdfs:subClassOf', 'property', 0, NULL);
INSERT INTO node(id, is_class, label, tag, source_type, sid) VALUES(9, false, 'rdfs:subPropertyOf', 'property', 0, NULL);
INSERT INTO node(id, is_class, label, tag, source_type, sid) VALUES(10, false, 'rdfs:domain', 'property', 0, NULL);
INSERT INTO node(id, is_class, label, tag, source_type, sid) VALUES(11, false, 'rdfs:range', 'property', 0, NULL);
INSERT INTO node(id, is_class, label, tag, source_type, sid) VALUES(12, false, 'rdfs:label', 'property', 0, NULL);
INSERT INTO node(id, is_class, label, tag, source_type, sid) VALUES(13, false, 'rdfs:comment', 'property', 0, NULL);
INSERT INTO node(id, is_class, label, tag, source_type, sid) VALUES(14, false, 'rdfs:seeAlso', 'property', 0, NULL);
INSERT INTO node(id, is_class, label, tag, source_type, sid) VALUES(15, false, 'rdfs:isDefinedBy', 'property', 0, NULL);

-- --------------------------------------------------------
-- graph stored procedures
-- --------------------------------------------------------

CREATE OR REPLACE PROCEDURE populate_node_class(instance_id integer, a_id integer, sub_id integer)
 LANGUAGE sql
AS $function$
    INSERT INTO node_class(node_id, class_id, "level")
    (WITH RECURSIVE cn AS (
        -- ANCHOR
        -- get object class node
        SELECT t.o_id AS id, 1 AS "level" FROM node n
        -- of a triple having S=start node P=a O=class node
        INNER JOIN triple t ON t.s_id=n.id AND t.p_id=a_id
        LEFT JOIN node n2 ON t.o_id=n2.id AND n2.is_class=true
        WHERE n.id=instance_id
        UNION DISTINCT
        -- RECURSIVE
        SELECT t.o_id AS id, "level"+1 AS "level" FROM cn
        -- sub_id is the ID of rdfs:subClassOf
        INNER JOIN triple t ON t.s_id=cn.id AND t.p_id=sub_id
        LEFT JOIN node n2 ON t.o_id=n2.id AND n2.is_class=true
    )
    SELECT instance_id, cn.id, cn.level FROM cn WHERE cn.id IS NOT NULL);
$function$
;
