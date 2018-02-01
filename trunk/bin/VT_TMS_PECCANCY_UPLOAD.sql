CREATE OR REPLACE VIEW VT_TMS_PECCANCY_UPLOAD AS
SELECT   t.XH,
         t.fxbh,
         t.sbbj,
         t.cdbh,
            decode(t.HPZL,'01',3,'02',3,'23',4,'24',5,'25',4,'13',6,'14',6,3) AS CLFL,
            decode(t.HPZL,'23','02','24','02','05','02','25','07','26','07',HPZL) AS HPZL,
            t.HPHM,
          NVL(T.CLLX,decode(t.HPZL,'13','H41','K31')) AS JTFS,
	      	NVL(T.FZJG,'ÉÂK')AS FZJG,
            decode(t.SJLY,'1',1,'2',2,'3',3,'4',4,'5',5,'6',6,'8',9,'9',9)AS SJLY,
            t.WFSJ,
          
         	nvl(F_GET_VALUE('fxcddbh','t_tgs_location','ddbh||ssxq',t.WFDD||t.CJJG),
         F_GET_VALUE('fxcddbh','t_tgs_location','ddbh',t.WFDD))AS WFDD,
         F_GET_VALUE('sbbh','t_tgs_location','ddbh',t.WFDD))AS SBBH,
			   nvl(t.WFDZ,F_GET_VALUE('ddmc','t_tgs_location','ddbh',t.WFDD)) as wfdz,
            t.WFXW,
            t.CSBJ,
       CJJG,
         f_get_value ('WFXWMS','T_TMS_PECCNACY_TYPE','WFXW',t.WFXW) AS WFNR,
        '' as zqmj,
				nvl(t.jdssyr,'Î´Öª')as jdssyr,
			  t.dh,
				t.lxfs,
				t.zsxxdz,
                t.zsxzqh,
	    nvl(F_GET_VALUE('xzqh','t_tgs_location','ddbh',t.WFDD),substr(t.cjjg,0,6)) as xzqh,
				t.clpp,
				t.cllx,
				t.csys,
				t.fdjh,
				t.clsbdh,
        t.clsd,
        F_GET_LIMITSPEED(t.wfdd,t.hpzl) as clxs,
        t.jcbj,
        NVL(t.SYXZ,'A') AS SYXZ
        (select 'http://'||zjwjip||zjwjlj  from  tgs_admin.t_tgs_capture where xh = t.xh and zjwjsx=1) as jpgfilename1,
        (select 'http://'||zjwjip||zjwjlj  from  tgs_admin.t_tgs_capture where xh = t.xh and zjwjsx=1) as jpgfilename2,
        (select 'http://'||zjwjip||zjwjlj  from  tgs_admin.t_tgs_capture where xh = t.xh and zjwjsx=1) as jpgfilename3
        
        
     FROM   t_tms_peccancy t
    WHERE
     t.CSBJ = '0'
     --and t.shbj='1' and jcbj='1'
     and shsj<=sysdate -40/24;
