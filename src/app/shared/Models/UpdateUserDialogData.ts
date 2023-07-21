import { Observable } from 'rxjs';
import { UserDto } from "../DTOs/UserDto";
import { User } from "./User";

export interface UpdateUserDialogData {
    user: User;
    updateUser: (userDto: UserDto) => Observable<User>;
  }
  