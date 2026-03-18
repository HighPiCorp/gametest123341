const defaultState = () => {
  return {
    login: '',
    testing: false,
    showMore: true,
    balance: 0,
    balanceString: null,
    activeCase: 0,
    mainMenu: true,
    giftMenu: false,
    twistCaseMenu: false,
    yourPrize: false,
    loader: false,
    myGifts: [],
    shuffleCase: [], //arritemsFromCases
    fastSpeen: false,
    speen: false,
    bpRewardLVL: 0,
    bpType:0,
    bpSeason: null,
    prize: {
      rare: 'legendary', item: '', name: 'BMW', id: 0, prize: '', tempItem: '', isCar: false
    },
    prizeBase: {
      id: null, rare: '', name: '', price: null, item: '', prize: '', tempItem: '', isCar: false
    },
    lastPrizes: [],
    cases: [],
  };
}

const roulette = new Vue({
  el: '#app',
  data() {
    return defaultState();
  },
  mounted() {

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //this.test();   /////////////////////////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////



    // this.balanceString = String(this.balance)
    //   .replace(/(\d{1,3}(?=(?:\d\d\d)+(?!\d)))/g, '$1.');

    if (!window.mp) {
      window.mp = {
        trigger() {
        },
        events: {
          add() {
          }
        }
      };
    }

    mp.events.add('CEF::battlePassRoulette:updatePlayerPrizesList', (info) => {
      const data = JSON.parse(info);

      if (data.hasOwnProperty('list')) roulette.myGifts = [...roulette.myGifts, ...data.list];
      if (data.hasOwnProperty('button')) roulette.showMore = data.button;
    });

    mp.events.add('CEF::battlePassRoulette:open', (info) => {
      const data = JSON.parse(info);

      if (data.hasOwnProperty('login')) roulette.login = data.login;
      if (data.hasOwnProperty('cases')) roulette.cases = data.cases;
      // if (data.hasOwnProperty('shuffleCasesList')) roulette.shuffleCasesList = data.shuffleCasesList;
      if (data.hasOwnProperty('balance')) roulette.balance = data.balance;
      if (data.hasOwnProperty('lastPrizes')) roulette.lastPrizes = data.lastPrizes;
      if (data.hasOwnProperty('activeCase')) roulette.activeCase = data.activeCase;
      if (data.hasOwnProperty('bpRewardLVL')) roulette.bpRewardLVL = data.bpRewardLVL;
      if (data.hasOwnProperty('bpType')) roulette.bpType = data.bpType;
      if (data.hasOwnProperty('bpSeason')) roulette.bpSeason = data.bpSeason;
    });

    mp.events.add('CEF::battlePassRoulette:preloadCase', (info) => {
      const data = JSON.parse(info);
      if (data.hasOwnProperty('shuffleCase')) {
        roulette.shuffleCase = data.shuffleCase;
      if (data.hasOwnProperty('activeCase'))
        roulette.activeCase = data.activeCase;
        setTimeout(() => {
          $('.arrow-case').css('left', `${(window.scrollX + window.innerWidth / 2 - 5) - 50}px`);
          //$('.arrow-case').css('top', `${(window.scrollY + window.innerHeight / 2 - 5) - 336}px`);
        }, 1);

        this.twistCaseMenu = true;
        this.giftMenu = false;
        this.mainMenu = false;
      }
    });

    mp.events.add('CEF::battlePassRoulette:speenCase', (index, winItem) => {
      this.fakeSpeen(index, winItem);
    });
  },
  methods: {
    test() {
      this.testing = true;
      this.cases = [
          {"name":"Silver","nameRU":"Серебрянный","price":250,"discountPrice":250,"discount":0,"percentCommon":355,"percentUncommon":122,"percentRare":20,"percentEpic":1,"percentLegendary":1,"percentFakeCommon":1,"percentFakeUncommon":1,"percentFakeRare":1,"percentFakeEpic":1,"percentFakeLegendary":1,"category":[{"rare":"common","items":["emperor","asea","dynasty"]},{"rare":"uncommon","items":["cash","manana","silverVip"]},{"rare":"rare","items":["rebel2","glendale2","armylicense","cheburek","patriot","cash"]},{"rare":"epic","items":["platinumVip","felon"]},{"rare":"legendary","items":["camry18","bmwm4","rmod240sx","bnr34","benzsl63","volvoxc90"]},{"rare":"fakeCommon","items":["apriora","ae86"]},{"rare":"fakeUncommon","items":["w210"]},{"rare":"fakeRare","items":["mark2","rmodm5e34","octavia18"]},{"rare":"fakeEpic","items":["cash","issi2","retinue","granger","sentinel3","zr350","s600","subie","optima","lancer","m3e30","bnr32","supra"]},{"rare":"fakeLegendary","items":["glc2021","c63coupe","lc200","m5"]}]},{"name":"Gold","nameRU":"Золотой","price":400,"discountPrice":400,"discount":0,"percentCommon":355,"percentUncommon":122,"percentRare":20,"percentEpic":1,"percentLegendary":1,"percentFakeCommon":1,"percentFakeUncommon":1,"percentFakeRare":1,"percentFakeEpic":1,"percentFakeLegendary":1,"category":[{"rare":"common","items":["silverVip","glendale","stanier"]},{"rare":"uncommon","items":["cash","goldVip","washington","armylicense"]},{"rare":"rare","items":["platinumVip","gauntlet"]},{"rare":"epic","items":["cash","sultan","caracara2","kamacho","jackal"]},{"rare":"legendary","items":["teslas","mcls53","rmodrs6"]},{"rare":"fakeCommon","items":["benzc32","gcmcentra20"]},{"rare":"fakeUncommon","items":["m3e60","lrdef17"]},{"rare":"fakeRare","items":["cash","asterope","seminole","rmodbiposto","golf7r","190e"]},{"rare":"fakeEpic","items":["rmodm3e36","rmodz350pandem","svt00","z48","audia6","m5e60","kiastinger"]},{"rare":"fakeLegendary","items":["rmodm4gts","durango18","g65fresh"]}]},{"name":"Diamond","nameRU":"Алмазный","price":600,"discountPrice":600,"discount":0,"percentCommon":355,"percentUncommon":122,"percentRare":20,"percentEpic":1,"percentLegendary":1,"percentFakeCommon":1,"percentFakeUncommon":1,"percentFakeRare":1,"percentFakeEpic":1,"percentFakeLegendary":1,"category":[{"rare":"common","items":["goldVip","platinumVip","cash","oracle2","gauntlet3","futo2"]},{"rare":"uncommon","items":["cash","mesa","retinue2","savestra"]},{"rare":"rare","items":["bjxl"]},{"rare":"epic","items":["sabregt"]},{"rare":"legendary","items":["rmodgt63","p1","f12berlinetta"]},{"rare":"fakeCommon","items":["audia8","chevelle1970","370z16","boss302","gl450"]},{"rare":"fakeUncommon","items":["bmwg20","rmodfordgt","drafter"]},{"rare":"fakeRare","items":["rmodsuprapamdem"]},{"rare":"fakeEpic","items":["v447","mbgls63"]},{"rare":"fakeLegendary","items":["chiron17","mbbs20"]}]}
      ];

      this.balance = 1000;

      this.shuffleCase = [
        {"item":"futo2","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"cash","rare":"common"},{"item":"retinue2","rare":"uncommon"},{"item":"gauntlet3","rare":"common"},{"item":"futo2","rare":"common"},{"item":"mesa","rare":"uncommon"},{"item":"oracle2","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"cash","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"cash","rare":"common"},{"item":"savestra","rare":"uncommon"},{"item":"retinue2","rare":"uncommon"},{"item":"futo2","rare":"common"},{"item":"futo2","rare":"common"},{"item":"mesa","rare":"uncommon"},{"item":"gauntlet3","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"mesa","rare":"uncommon"},{"item":"futo2","rare":"common"},{"item":"retinue2","rare":"uncommon"},{"item":"cash","rare":"uncommon"},{"item":"oracle2","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"savestra","rare":"uncommon"},{"item":"gauntlet3","rare":"common"},{"item":"cash","rare":"uncommon"},{"item":"gauntlet3","rare":"common"},{"item":"futo2","rare":"common"},{"item":"futo2","rare":"common"},{"item":"mesa","rare":"uncommon"},{"item":"cash","rare":"common"},{"item":"cash","rare":"uncommon"},{"item":"platinumVip","rare":"common"},{"item":"futo2","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"savestra","rare":"uncommon"},{"item":"savestra","rare":"uncommon"},{"item":"platinumVip","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"cash","rare":"common"},{"item":"futo2","rare":"common"},{"item":"cash","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"mbgls63","rare":"fakeEpic"},{"item":"cash","rare":"uncommon"},{"item":"savestra","rare":"uncommon"},{"item":"oracle2","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"cash","rare":"common"},{"item":"cash","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"retinue2","rare":"uncommon"},{"item":"futo2","rare":"common"},{"item":"futo2","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"retinue2","rare":"uncommon"},{"item":"futo2","rare":"common"},{"item":"futo2","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"savestra","rare":"uncommon"},{"item":"futo2","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"futo2","rare":"common"},{"item":"cash","rare":"uncommon"},{"item":"goldVip","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"savestra","rare":"uncommon"},{"item":"oracle2","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"savestra","rare":"uncommon"},{"item":"futo2","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"futo2","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"bjxl","rare":"rare"},{"item":"oracle2","rare":"common"},{"item":"bjxl","rare":"rare"},{"item":"retinue2","rare":"uncommon"},{"item":"cash","rare":"common"},{"item":"cash","rare":"uncommon"},{"item":"oracle2","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"bjxl","rare":"rare"},{"item":"mesa","rare":"uncommon"},{"item":"oracle2","rare":"common"},{"item":"futo2","rare":"common"},{"item":"cash","rare":"common"},{"item":"savestra","rare":"uncommon"},{"item":"goldVip","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"chiron17","rare":"fakeLegendary"},{"item":"platinumVip","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"futo2","rare":"common"},{"item":"cash","rare":"uncommon"},{"item":"goldVip","rare":"common"},{"item":"mesa","rare":"uncommon"},{"item":"futo2","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"cash","rare":"uncommon"},{"item":"platinumVip","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"savestra","rare":"uncommon"},{"item":"cash","rare":"uncommon"},{"item":"savestra","rare":"uncommon"},{"item":"gauntlet3","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"futo2","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"savestra","rare":"uncommon"},{"item":"bjxl","rare":"rare"},{"item":"boss302","rare":"fakeCommon"},{"item":"futo2","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"cash","rare":"uncommon"},{"item":"goldVip","rare":"common"},{"item":"mesa","rare":"uncommon"},{"item":"oracle2","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"cash","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"savestra","rare":"uncommon"},{"item":"futo2","rare":"common"},{"item":"cash","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"futo2","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"savestra","rare":"uncommon"},{"item":"savestra","rare":"uncommon"},{"item":"futo2","rare":"common"},{"item":"cash","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"cash","rare":"common"},{"item":"futo2","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"bjxl","rare":"rare"},{"item":"platinumVip","rare":"common"},{"item":"cash","rare":"common"},{"item":"retinue2","rare":"uncommon"},{"item":"gauntlet3","rare":"common"},{"item":"retinue2","rare":"uncommon"},{"item":"goldVip","rare":"common"},{"item":"cash","rare":"uncommon"},{"item":"mesa","rare":"uncommon"},{"item":"cash","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"cash","rare":"common"},{"item":"cash","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"bjxl","rare":"rare"},{"item":"oracle2","rare":"common"},{"item":"cash","rare":"common"},{"item":"mesa","rare":"uncommon"},{"item":"savestra","rare":"uncommon"},{"item":"goldVip","rare":"common"},{"item":"retinue2","rare":"uncommon"},{"item":"futo2","rare":"common"},{"item":"bjxl","rare":"rare"},{"item":"cash","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"cash","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"cash","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"cash","rare":"uncommon"},{"item":"gauntlet3","rare":"common"},{"item":"savestra","rare":"uncommon"},{"item":"retinue2","rare":"uncommon"},{"item":"goldVip","rare":"common"},{"item":"cash","rare":"common"},{"item":"futo2","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"cash","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"mesa","rare":"uncommon"},{"item":"platinumVip","rare":"common"},{"item":"cash","rare":"common"},{"item":"cash","rare":"common"},{"item":"cash","rare":"uncommon"},{"item":"gauntlet3","rare":"common"},{"item":"mesa","rare":"uncommon"},{"item":"goldVip","rare":"common"},{"item":"retinue2","rare":"uncommon"},{"item":"rmodsuprapandem","rare":"fakeRare"},{"item":"gauntlet3","rare":"common"},{"item":"bjxl","rare":"rare"},{"item":"oracle2","rare":"common"},{"item":"cash","rare":"common"},{"item":"futo2","rare":"common"},{"item":"cash","rare":"common"},{"item":"cash","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"bjxl","rare":"rare"},{"item":"futo2","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"mesa","rare":"uncommon"},{"item":"savestra","rare":"uncommon"},{"item":"futo2","rare":"common"},{"item":"mesa","rare":"uncommon"},{"item":"oracle2","rare":"common"},{"item":"retinue2","rare":"uncommon"},{"item":"retinue2","rare":"uncommon"},{"item":"futo2","rare":"common"},{"item":"bjxl","rare":"rare"},{"item":"gauntlet3","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"cash","rare":"uncommon"},{"item":"oracle2","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"futo2","rare":"common"},{"item":"cash","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"retinue2","rare":"uncommon"},{"item":"platinumVip","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"savestra","rare":"uncommon"},{"item":"oracle2","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"cash","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"mesa","rare":"uncommon"},{"item":"platinumVip","rare":"common"},{"item":"mesa","rare":"uncommon"},{"item":"futo2","rare":"common"},{"item":"cash","rare":"uncommon"},{"item":"bjxl","rare":"rare"},{"item":"bjxl","rare":"rare"},{"item":"platinumVip","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"savestra","rare":"uncommon"},{"item":"futo2","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"cash","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"mesa","rare":"uncommon"},{"item":"retinue2","rare":"uncommon"},{"item":"mesa","rare":"uncommon"},{"item":"platinumVip","rare":"common"},{"item":"cash","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"bjxl","rare":"rare"},{"item":"savestra","rare":"uncommon"},{"item":"mesa","rare":"uncommon"},{"item":"bjxl","rare":"rare"},{"item":"goldVip","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"savestra","rare":"uncommon"},{"item":"goldVip","rare":"common"},{"item":"cash","rare":"common"},{"item":"cash","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"savestra","rare":"uncommon"},{"item":"platinumVip","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"cash","rare":"common"},{"item":"cash","rare":"common"},{"item":"futo2","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"savestra","rare":"uncommon"},{"item":"goldVip","rare":"common"},{"item":"futo2","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"cash","rare":"common"},{"item":"mesa","rare":"uncommon"},{"item":"gauntlet3","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"cash","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"retinue2","rare":"uncommon"},{"item":"cash","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"savestra","rare":"uncommon"},{"item":"platinumVip","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"cash","rare":"common"},{"item":"cash","rare":"uncommon"},{"item":"bjxl","rare":"rare"},{"item":"retinue2","rare":"uncommon"},{"item":"futo2","rare":"common"},{"item":"savestra","rare":"uncommon"},{"item":"mesa","rare":"uncommon"},{"item":"oracle2","rare":"common"},{"item":"futo2","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"futo2","rare":"common"},{"item":"savestra","rare":"uncommon"},{"item":"oracle2","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"mesa","rare":"uncommon"},{"item":"gauntlet3","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"retinue2","rare":"uncommon"},{"item":"cash","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"retinue2","rare":"uncommon"},{"item":"gauntlet3","rare":"common"},{"item":"cash","rare":"common"},{"item":"cash","rare":"uncommon"},{"item":"futo2","rare":"common"},{"item":"cash","rare":"common"},{"item":"p1","rare":"legendary"},{"item":"oracle2","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"cash","rare":"common"},{"item":"savestra","rare":"uncommon"},{"item":"futo2","rare":"common"},{"item":"retinue2","rare":"uncommon"},{"item":"cash","rare":"uncommon"},{"item":"oracle2","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"mesa","rare":"uncommon"},{"item":"oracle2","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"retinue2","rare":"uncommon"},{"item":"platinumVip","rare":"common"},{"item":"cash","rare":"uncommon"},{"item":"gauntlet3","rare":"common"},{"item":"drafter","rare":"fakeUncommon"},{"item":"platinumVip","rare":"common"},{"item":"bjxl","rare":"rare"},{"item":"retinue2","rare":"uncommon"},{"item":"gauntlet3","rare":"common"},{"item":"futo2","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"cash","rare":"uncommon"},{"item":"gauntlet3","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"cash","rare":"uncommon"},{"item":"cash","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"mesa","rare":"uncommon"},{"item":"bjxl","rare":"rare"},{"item":"cash","rare":"uncommon"},{"item":"goldVip","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"retinue2","rare":"uncommon"},{"item":"savestra","rare":"uncommon"},{"item":"cash","rare":"uncommon"},{"item":"cash","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"cash","rare":"uncommon"},{"item":"goldVip","rare":"common"},{"item":"futo2","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"bjxl","rare":"rare"},{"item":"futo2","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"futo2","rare":"common"},{"item":"retinue2","rare":"uncommon"},{"item":"oracle2","rare":"common"},{"item":"cash","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"mesa","rare":"uncommon"},{"item":"mesa","rare":"uncommon"},{"item":"futo2","rare":"common"},{"item":"cash","rare":"uncommon"},{"item":"cash","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"cash","rare":"epic"},{"item":"cash","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"cash","rare":"uncommon"},{"item":"cash","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"retinue2","rare":"uncommon"},{"item":"gauntlet3","rare":"common"},{"item":"cash","rare":"common"},{"item":"bjxl","rare":"rare"},{"item":"futo2","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"savestra","rare":"uncommon"},{"item":"savestra","rare":"uncommon"},{"item":"oracle2","rare":"common"},{"item":"cash","rare":"common"},{"item":"futo2","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"futo2","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"savestra","rare":"uncommon"},{"item":"bjxl","rare":"rare"},{"item":"oracle2","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"savestra","rare":"uncommon"},{"item":"cash","rare":"common"},{"item":"mesa","rare":"uncommon"},{"item":"cash","rare":"uncommon"},{"item":"goldVip","rare":"common"},{"item":"mesa","rare":"uncommon"},{"item":"retinue2","rare":"uncommon"},{"item":"retinue2","rare":"uncommon"},{"item":"oracle2","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"futo2","rare":"common"},{"item":"cash","rare":"uncommon"},{"item":"goldVip","rare":"common"},{"item":"futo2","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"mesa","rare":"uncommon"},{"item":"oracle2","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"futo2","rare":"common"},{"item":"cash","rare":"uncommon"},{"item":"platinumVip","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"futo2","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"goldVip","rare":"common"},{"item":"futo2","rare":"common"},{"item":"cash","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"gauntlet3","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"retinue2","rare":"uncommon"},{"item":"cash","rare":"uncommon"},{"item":"cash","rare":"common"},{"item":"futo2","rare":"common"},{"item":"mesa","rare":"uncommon"},{"item":"savestra","rare":"uncommon"},{"item":"goldVip","rare":"common"},{"item":"oracle2","rare":"common"},{"item":"platinumVip","rare":"common"},{"item":"cash","rare":"common"},{"item":"futo2","rare":"common"},{"item":"mesa","rare":"uncommon"},{"item":"gauntlet3","rare":"common"}];

      this.lastPrizes = [
        {"item":"mark2","rare":"epic","name":"ToyotaMarkII","date":"Ср,30.11.20220:00:00","tempItem":"car"},{"item":"bfinjection","rare":"rare","name":"BFInjection","date":"Ср,30.11.20220:00:00","tempItem":"car"},{"item":"faction2","rare":"rare","name":"WillardFactionCustom","date":"Ср,30.11.20220:00:00","tempItem":"car"},{"item":"silverVip","rare":"uncommon","name":"SilverVIP(15дней)","date":"Ср,30.11.20220:00:00","tempItem":"silverVip"},{"item":"silverVip","rare":"common","name":"SilverVIP(15дней)","date":"Ср,30.11.20220:00:00","tempItem":"silverVip"},{"item":"silverVip","rare":"common","name":"SilverVIP(15дней)","date":"Ср,30.11.20220:00:00","tempItem":"silverVip"},{"item":"silverVip","rare":"uncommon","name":"SilverVIP(15дней)","date":"Ср,30.11.20220:00:00","tempItem":"silverVip"},{"item":"clothes","rare":"common","name":"одежду","date":"Ср,30.11.20220:00:00","tempItem":"clothes"},{"item":"brioso","rare":"uncommon","name":"GrottiBriosoR/A","date":"Ср,30.11.20220:00:00","tempItem":"car"},{"item":"manana","rare":"common","name":"AlbanyMananaCabrio","date":"Ср,30.11.20220:00:00","tempItem":"car"}
      ];

      this.myGifts = [
        {"id":530,"uuid":333333,"item":"silverVip","rare":"uncommon","prize":"2","price":200,"name":"SilverVIP(30дней)","isTaked":false,"isSold":false,"isLocked":false,"date":"Пт,09.12.20220:00:00"},{"id":529,"uuid":333333,"item":"asea","rare":"common","prize":"asea","price":120,"name":"DeclasseAsea","isTaked":false,"isSold":false,"isLocked":false,"date":"Пт,09.12.20220:00:00"},{"id":528,"uuid":333333,"item":"silverVip","rare":"uncommon","prize":"2","price":200,"name":"SilverVIP(30дней)","isTaked":false,"isSold":false,"isLocked":false,"date":"Пт,09.12.20220:00:00"},{"id":527,"uuid":333333,"item":"cash","rare":"uncommon","prize":"20000","price":0,"name":"20000$","isTaked":false,"isSold":false,"isLocked":false,"date":"Пт,09.12.20220:00:00"},{"id":526,"uuid":333333,"item":"asea","rare":"common","prize":"asea","price":120,"name":"DeclasseAsea","isTaked":false,"isSold":true,"isLocked":true,"date":"Пт,09.12.20220:00:00"},{"id":525,"uuid":333333,"item":"manana","rare":"uncommon","prize":"manana","price":189,"name":"AlbanyMananaCabrio","isTaked":false,"isSold":false,"isLocked":false,"date":"Пт,09.12.20220:00:00"},{"id":524,"uuid":333333,"item":"asea","rare":"common","prize":"asea","price":120,"name":"DeclasseAsea","isTaked":false,"isSold":false,"isLocked":false,"date":"Пт,09.12.20220:00:00"},{"id":523,"uuid":333333,"item":"emperor","rare":"common","prize":"emperor","price":110,"name":"AlbanyEmperor","isTaked":false,"isSold":false,"isLocked":false,"date":"Пт,09.12.20220:00:00"},{"id":522,"uuid":333333,"item":"dynasty","rare":"common","prize":"dynasty","price":120,"name":"WeenyDynasty","isTaked":false,"isSold":false,"isLocked":false,"date":"Пт,09.12.20220:00:00"},{"id":521,"uuid":333333,"item":"emperor","rare":"common","prize":"emperor","price":110,"name":"AlbanyEmperor","isTaked":false,"isSold":false,"isLocked":false,"date":"Пт,09.12.20220:00:00"},{"id":520,"uuid":333333,"item":"emperor","rare":"common","prize":"emperor","price":110,"name":"AlbanyEmperor","isTaked":false,"isSold":false,"isLocked":false,"date":"Пт,09.12.20220:00:00"},{"id":519,"uuid":333333,"item":"asea","rare":"common","prize":"asea","price":120,"name":"DeclasseAsea","isTaked":false,"isSold":false,"isLocked":false,"date":"Пт,09.12.20220:00:00"},{"id":518,"uuid":333333,"item":"dynasty","rare":"common","prize":"dynasty","price":120,"name":"WeenyDynasty","isTaked":false,"isSold":false,"isLocked":false,"date":"Пт,09.12.20220:00:00"},{"id":517,"uuid":333333,"item":"asea","rare":"common","prize":"asea","price":120,"name":"DeclasseAsea","isTaked":false,"isSold":false,"isLocked":false,"date":"Пт,09.12.20220:00:00"},{"id":516,"uuid":333333,"item":"emperor","rare":"common","prize":"emperor","price":110,"name":"AlbanyEmperor","isTaked":false,"isSold":false,"isLocked":false,"date":"Пт,09.12.20220:00:00"},{"id":515,"uuid":333333,"item":"asea","rare":"common","prize":"asea","price":120,"name":"DeclasseAsea","isTaked":false,"isSold":false,"isLocked":false,"date":"Пт,09.12.20220:00:00"},{"id":514,"uuid":333333,"item":"asea","rare":"common","prize":"asea","price":120,"name":"DeclasseAsea","isTaked":false,"isSold":false,"isLocked":false,"date":"Пт,09.12.20220:00:00"},{"id":74,"uuid":333333,"item":"patriot","rare":"rare","prize":"patriot","price":784,"name":"MammothPatriot","isTaked":false,"isSold":true,"isLocked":true,"date":"Пт,09.12.20220:00:00"},{"id":73,"uuid":333333,"item":"futo2","rare":"common","prize":"futo2","price":427,"name":"KarinFutoGTX","isTaked":false,"isSold":true,"isLocked":true,"date":"Пн,05.12.20220:00:00"},{"id":72,"uuid":333333,"item":"mesa","rare":"uncommon","prize":"mesa","price":560,"name":"CanisMesa","isTaked":false,"isSold":true,"isLocked":true,"date":"Пн,05.12.20220:00:00"},{"id":71,"uuid":333333,"item":"gauntlet3","rare":"common","prize":"gauntlet3","price":581,"name":"BravadoGauntletClassic","isTaked":false,"isSold":true,"isLocked":true,"date":"Пн,05.12.20220:00:00"},{"id":70,"uuid":333333,"item":"platinumVip","rare":"common","prize":"4","price":250,"name":"PlatinumVIP(15дней)","isTaked":false,"isSold":true,"isLocked":true,"date":"Пн,05.12.20220:00:00"},{"id":69,"uuid":333333,"item":"goldVip","rare":"common","prize":"3","price":150,"name":"GoldVIP(15дней)","isTaked":false,"isSold":true,"isLocked":true,"date":"Пн,05.12.20220:00:00"},{"id":68,"uuid":333333,"item":"oracle2","rare":"common","prize":"oracle2","price":490,"name":"ÜbermachtOracle","isTaked":false,"isSold":true,"isLocked":true,"date":"Пн,05.12.20220:00:00"},{"id":67,"uuid":333333,"item":"goldVip","rare":"common","prize":"3","price":150,"name":"GoldVIP(15дней)","isTaked":false,"isSold":true,"isLocked":true,"date":"Пн,05.12.20220:00:00"},{"id":66,"uuid":333333,"item":"mesa","rare":"uncommon","prize":"mesa","price":560,"name":"CanisMesa","isTaked":false,"isSold":true,"isLocked":true,"date":"Пн,05.12.20220:00:00"},{"id":65,"uuid":333333,"item":"cash","rare":"uncommon","prize":"100000","price":0,"name":"100000$","isTaked":false,"isSold":true,"isLocked":true,"date":"Пн,05.12.20220:00:00"},{"id":64,"uuid":333333,"item":"mesa","rare":"uncommon","prize":"mesa","price":560,"name":"CanisMesa","isTaked":false,"isSold":true,"isLocked":true,"date":"Пн,05.12.20220:00:00"},{"id":63,"uuid":333333,"item":"gauntlet3","rare":"common","prize":"gauntlet3","price":581,"name":"BravadoGauntletClassic","isTaked":false,"isSold":true,"isLocked":true,"date":"Пн,05.12.20220:00:00"},{"id":62,"uuid":333333,"item":"futo2","rare":"common","prize":"futo2","price":427,"name":"KarinFutoGTX","isTaked":false,"isSold":true,"isLocked":true,"date":"Пн,05.12.20220:00:00"},{"id":61,"uuid":333333,"item":"cash","rare":"common","prize":"50000","price":0,"name":"50000$","isTaked":false,"isSold":true,"isLocked":true,"date":"Пн,05.12.20220:00:00"},{"id":60,"uuid":333333,"item":"retinue2","rare":"uncommon","prize":"retinue2","price":840,"name":"VapidRetinueMkII","isTaked":false,"isSold":true,"isLocked":true,"date":"Пн,05.12.20220:00:00"},{"id":59,"uuid":333333,"item":"retinue2","rare":"uncommon","prize":"retinue2","price":840,"name":"VapidRetinueMkII","isTaked":false,"isSold":true,"isLocked":true,"date":"Пн,05.12.20220:00:00"},{"id":58,"uuid":333333,"item":"futo2","rare":"common","prize":"futo2","price":427,"name":"KarinFutoGTX","isTaked":false,"isSold":true,"isLocked":true,"date":"Пн,05.12.20220:00:00"},{"id":57,"uuid":333333,"item":"cash","rare":"common","prize":"50000","price":0,"name":"50000$","isTaked":false,"isSold":true,"isLocked":true,"date":"Пн,05.12.20220:00:00"},{"id":56,"uuid":333333,"item":"oracle2","rare":"common","prize":"oracle2","price":490,"name":"ÜbermachtOracle","isTaked":false,"isSold":true,"isLocked":true,"date":"Пн,05.12.20220:00:00"},{"id":55,"uuid":333333,"item":"futo2","rare":"common","prize":"futo2","price":427,"name":"KarinFutoGTX","isTaked":false,"isSold":true,"isLocked":true,"date":"Пн,05.12.20220:00:00"},{"id":54,"uuid":333333,"item":"cash","rare":"common","prize":"50000","price":0,"name":"50000$","isTaked":false,"isSold":true,"isLocked":true,"date":"Пн,05.12.20220:00:00"},{"id":53,"uuid":333333,"item":"gauntlet3","rare":"common","prize":"gauntlet3","price":581,"name":"BravadoGauntletClassic","isTaked":false,"isSold":true,"isLocked":true,"date":"Пн,05.12.20220:00:00"},{"id":52,"uuid":333333,"item":"cash","rare":"common","prize":"50000","price":0,"name":"50000$","isTaked":false,"isSold":true,"isLocked":true,"date":"Пн,05.12.20220:00:00"},{"id":51,"uuid":333333,"item":"oracle2","rare":"common","prize":"oracle2","price":490,"name":"ÜbermachtOracle","isTaked":false,"isSold":true,"isLocked":true,"date":"Пн,05.12.20220:00:00"},{"id":50,"uuid":333333,"item":"cash","rare":"common","prize":"50000","price":0,"name":"50000$","isTaked":false,"isSold":true,"isLocked":true,"date":"Пн,05.12.20220:00:00"},{"id":49,"uuid":333333,"item":"chiron17","rare":"fakeLegendary","prize":"chiron17","price":110250,"name":"BugattiChiron","isTaked":false,"isSold":true,"isLocked":true,"date":"Пн,05.12.20220:00:00"},{"id":48,"uuid":333333,"item":"gauntlet3","rare":"common","prize":"gauntlet3","price":581,"name":"BravadoGauntletClassic","isTaked":false,"isSold":true,"isLocked":true,"date":"Пн,05.12.20220:00:00"},{"id":47,"uuid":333333,"item":"futo2","rare":"common","prize":"futo2","price":427,"name":"KarinFutoGTX","isTaked":false,"isSold":true,"isLocked":true,"date":"Пн,05.12.20220:00:00"},{"id":46,"uuid":333333,"item":"cash","rare":"common","prize":"50000","price":0,"name":"50000$","isTaked":false,"isSold":true,"isLocked":true,"date":"Пн,05.12.20220:00:00"},{"id":45,"uuid":333333,"item":"gauntlet3","rare":"common","prize":"gauntlet3","price":581,"name":"BravadoGauntletClassic","isTaked":false,"isSold":true,"isLocked":true,"date":"Пн,05.12.20220:00:00"},{"id":44,"uuid":333333,"item":"platinumVip","rare":"common","prize":"4","price":250,"name":"PlatinumVIP(15дней)","isTaked":false,"isSold":true,"isLocked":true,"date":"Пн,05.12.20220:00:00"},{"id":43,"uuid":333333,"item":"platinumVip","rare":"common","prize":"4","price":250,"name":"PlatinumVIP(15дней)","isTaked":false,"isSold":true,"isLocked":true,"date":"Пн,05.12.20220:00:00"},{"id":42,"uuid":333333,"item":"bjxl","rare":"rare","prize":"bjxl","price":945,"name":"KarinBeeJayXL","isTaked":false,"isSold":true,"isLocked":true,"date":"Пн,05.12.20220:00:00"}];

      setTimeout(() => {
        $('.arrow-case').css('left', `${(window.scrollX + window.innerWidth / 2 - 5) - 50}px`);
        //$('.arrow-case').css('top', `${(window.scrollY + window.innerHeight / 2 - 5) - 336}px`);
      }, 1);

      this.twistCaseMenu = true;
      this.giftMenu = false;
      this.mainMenu = false;
    },

    goNextPrizes() {
      mp.trigger('CLIENT::battlePassRoulette:getNextPrizesList', roulette.myGifts[roulette.myGifts.length - 1].id);
    },

    speenCase() {
      if(this.speen) return;

      if (this.testing) {
        var fakeIndex = 257;
        var winItem = null;
        this.fakeSpeen(fakeIndex, winItem);
      }
      else mp.trigger("CLIENT::battlePassRoulette:trySpeenCase", this.activeCase, this.bpRewardLVL, this.bpType, this.bpSeason);
    },

    fakeSpeen(index, winItem) {
      // console.log("winItem: "+ winItem);
      // console.log(`fakeSpeen index: ${index} fastSpeen: ${this.fastSpeen} speen: ${this.speen}`);

      let anim = false;

      jQuery.easing['easeOutCirc'] = function (x, t, b, c, d) {
        return c * Math.sqrt(1 - (t = t / d - 1 ) * t ) + b;
      }

      if (!this.fastSpeen || this.fastSpeen === undefined && !this.speen) {
        this.speen = true;
        console.log("speen");

        const option = {
          speed: 2,
          duration: 13,
          stopIndex: index,
        };

        // Делаем анимацию.
        const listItems = $(".item-block-switch");
        const neededItem = $(listItems[index]);
        const itemOuterWidth = neededItem.outerWidth();
        const containerOuterWidth = itemOuterWidth * roulette.shuffleCase.length;
        const scrollLeftUpd = containerOuterWidth * option.speed + itemOuterWidth * (option.stopIndex - 2);

        $(".list-items-content").css({left:0}).animate({
          left: scrollLeftUpd, // До куда крутить.
        }, {
          duration: option.duration * 1000,
          easing: "easeOutCirc",
          step(now, fx) {
            $(".list-items-content").css('transform', `translateX(-${now % containerOuterWidth}px)`);
          },
          complete() {
            anim = false;

            if (!roulette.testing) {
              setTimeout(() => {
                roulette.twistCaseMenu = false;
                roulette.loader = true;

                setTimeout((_this) => {
                  roulette.itemPrize(winItem);
                  roulette.speen = false;
                }, 2000);
              }, 3000);
            }

            return anim;
          }
        })
      }

      if (!this.speen && this.fastSpeen) {
        console.log("fastSpeen");

        roulette.itemPrize(winItem);
      }
    },

    updateBalance(balance) {
      roulette.balance = balance;
    },

    setLastPrizes(data) {
      // Список лучших последних выигрышей
      this.lastPrizes = JSON.parse(JSON.stringify(data));
    },

    sellItem(id) {
      // ПРОДАЖА ITEM
      mp.trigger('CLIENT::battlePassRoulette:sellPrize', id);

      if(this.yourPrize)
      {
        this.mainMenu = true;
        this.yourPrize = false;
        this.giftMenu = false;
        this.twistCaseMenu = false;
      }

      this.myGifts = this.myGifts.filter((gift) => {
        if (gift.id === id) {
          gift.isSold = true;
        }
        return gift;
      });
    },

    takeItem(id) {
      // ПОЛУЧЕНИЕ ITEM
      mp.trigger('CLIENT::battlePassRoulette:takePlayerPrize', id);
      mp.trigger('CLIENT::HTML:close');
      this.mainMenu = true;
      this.yourPrize = false;
      this.giftMenu = false;
      this.twistCaseMenu = false;
      this.myGifts = this.myGifts.filter((gift) => {
        if (gift.id === id) {
          gift.isTaked = true;
        }
        return gift;
      });
    },

    itemPrize(prizeItem) {
      console.log("prizeItem: "+ prizeItem);
      const data = JSON.parse(prizeItem);

      const { id } = data.id;
      const { rare } = data.rare;
      const { name } = data.name;
      const { price } = data.price;
      const { item } = data.item;
      const { prize } = data.prize;
      const { tempItem } = data.tempItem;
      const { isCar } = data.isCar;

      this.prizeBase = {
        id, rare, name, price, item, prize, tempItem, isCar
      };

      this.prize.rare = data.rare;
      this.prize.id = data.id;
      this.prize.name = data.name;
      this.prize.price = data.price;
      this.prize.prize = data.prize;
      this.prize.item = data.item;
      this.prize.tempItem = data.tempItem;
      this.prize.isCar = data.isCar;
      this.loader = false;

      if (data.isCar === true && (data.rare === 'epic' || data.rare === 'fakeEpic' || data.rare === 'legendary'|| data.rare === 'fakeLegendary')) {
        mp.trigger("CLIENT::battlePassRoulette:ShowPrizeToAll", roulette.cases[roulette.activeCase].name, data.name);
      }


      this.yourPrize = true;
    },


    reloadPage() {
      this.mainMenu = true;
      this.yourPrize = false;
      this.giftMenu = false;
      this.twistCaseMenu = false;
    },

    switchCaseLeft() {
      if (this.activeCase) {
        this.activeCase -= 1;
      } else {
        this.activeCase = (this.cases.length - 1);
      }
    },

    switchCaseRight() {
      if (this.activeCase <= this.cases.length) {
        this.activeCase += 1;
        if (this.activeCase === this.cases.length) this.activeCase = 0;
      }
    },

    goGifts() {
      this.giftMenu = true;
      this.mainMenu = false;
      this.twistCaseMenu = false;

      console.log(this.myGifts.length);
      this.myGifts = [];
      this.showMore = true;
      mp.trigger('CLIENT::caseRoulette:getPlayerPrizes');
    },

    goHome() {
      this.yourPrize = false;
      this.giftMenu = false;
      this.twistCaseMenu = false;
      this.mainMenu = true;
    },

    goAddCoin() {
      mp.trigger("CLIENT::stat:addCoin", this.login);
    },

    tryOpenCase() {
      mp.trigger("CLIENT::battlePassRoulette:tryOpenCase", this.activeCase);
    },
  },
});
