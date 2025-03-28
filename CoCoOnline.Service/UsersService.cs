using CoCoOnline.Models;
using CoCoOnline.Repository;
using CoCoOnline.Uitles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CoCoOnline.Service
{
    public class UsersService
    {
        private readonly UsersRepository usersRepository;

        public UsersService(UsersRepository usersRepository)
        {
            this.usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
        }

        public ReturnData<Users?> Login(string LoginName, string UserPWD)
        {
            return usersRepository.Login(LoginName, UserPWD);
        }
        public ReturnData<Users?> AdminLogin(string LoginName, string UserPWD)
        {
            return usersRepository.AdminLogin(LoginName, UserPWD);
        }        
        public ReturnData<Users?> Register(Users user)
        {
            ReturnData<Users?> returnData = new ReturnData<Users?>(0);

            if (usersRepository.Insert(user) > 0)
            {
                returnData.IntResult = 1;
            }
            else
            {
                returnData.IntResult = 0;
            }
            return returnData;
        }
        public ReturnData<Users?> Update(Users user)
        {
            ReturnData<Users?> returnData = new ReturnData<Users?>(0);
            if (usersRepository.Update(user) > 0)
            {
                returnData.IntResult = 1;
            }
            else
            {
                returnData.IntResult = 0;
            }
            return returnData;            
        }
        public List<Users> GetList()
        {
            return usersRepository.GetList();
        }
        public List<Users> GetList(Expression<Func<Users, bool>> predicate)
        {
            return usersRepository.GetList(predicate);
        }
        public Users GetUserByLoginName(string LoginName)
        {
            return usersRepository.GetUserByLoginName(LoginName);
        }
        public IQueryable<Users> GetPagedList(int pageIndex, int pageSize, out int totalCount,
            Expression<Func<Users, bool>> predicate,
            Expression<Func<Users, object>> orderBy, bool isAscending = true)
        {
            return usersRepository.GetPagedList(pageIndex, pageSize, out totalCount,predicate, orderBy, isAscending);
        }

        public ReturnData<Users?> Delete(string userids)
        {            
            return usersRepository.Delete(userids);
        }
    }
}
